# Architecture Diagrams

## 1. High-Level Message Flow

```
┌──────────────────────────────────────────────────────────────────┐
│                  Azure Service Bus Topic                         │
│  (Messages from multiple sources: SourceA, SourceB, etc.)       │
└───────────────────────────┬──────────────────────────────────────┘
							│
							│ Message with Source ID in
							│ ApplicationProperties["Source"]
							│
							▼
		┌─────────────────────────────────────────────────┐
		│  AzureServiceBusConsumerService (Background)   │
		│                                                 │
		│  1. Receives message from Service Bus           │
		│  2. Extracts source: ExtractMessageSource()     │
		│  3. Maps using factory: MapperFactory           │
		│  4. Passes to CompanyMessageProcessor           │
		└──────────────┬────────────────────────────────┘
					   │
					   │ (source, rawMessage)
					   ▼
		┌──────────────────────────────────────────────┐
		│         IMapperFactory                       │
		│     (Factory Pattern)                        │
		│                                              │
		│  - Maintains registry of mappers            │
		│  - O(1) mapper selection by source          │
		│  - Delegates transformation to mapper       │
		└──────────┬────────────────────────────────┬─┘
				   │                                │
	 ┌─────────────┴────────────────────────────┬──┴──────────┐
	 │                                          │             │
	 ▼                                          ▼             ▼
┌──────────────┐  ┌───────────────┐  ┌──────────────────────┐
│ SourceAMapper│  │ SourceBMapper │  │ Direct (no mapping)  │
│(IMessageMapper)│ │(IMessageMapper)│ │(Deserialize directly)│
│              │  │               │  │                      │
│ Strategy     │  │ Strategy      │  │ Strategy             │
│ Pattern      │  │ Pattern       │  │ Pattern              │
└──────────────┘  └───────────────┘  └──────────────────────┘
	 │                   │                     │
	 │ Map()             │ Map()                │ Map()
	 │                   │                     │
	 └─────────────────┬─────────────────────┬─┘
					   │
					   ▼
		┌──────────────────────────────────────┐
		│    CompanyInputDto (Unified Schema)  │
		│                                      │
		│  • Name                              │
		│  • Address                           │
		│  • City                              │
		│  • Country                           │
		│  • Phone                             │
		│  • ... (other required fields)       │
		└─────────────┬───────────────────────┘
					  │
					  ▼
		┌──────────────────────────────────────┐
		│  CompanyMessageProcessor             │
		│                                      │
		│  - Validates required fields         │
		│  - Handles duplicates                │
		│  - Tracks processing results         │
		└─────────────┬───────────────────────┘
					  │
					  ▼
		┌──────────────────────────────────────┐
		│  AddCompanyEventHandler              │
		│                                      │
		│  - Checks for duplicates             │
		│  - Creates Company entity            │
		│  - Persists to database              │
		└─────────────┬───────────────────────┘
					  │
					  ▼
		┌──────────────────────────────────────┐
		│     SQL Database                     │
		│     Companies Table                  │
		│                                      │
		│  [Successfully stored]               │
		└──────────────────────────────────────┘
```

## 2. Mapper Selection Logic

```
					Message arrives
						   │
						   ▼
				  Has "Source" property?
					/              \
				  Yes              No
				   │                │
				   ▼                ▼
			Extract source    Use "Subject"
				   │                │
				   └────────┬───────┘
							│
					Source identifier
					(e.g., "SourceA")
							│
							▼
			MapperFactory.GetMapper(source)
							│
				┌───────────┼───────────┐
				│           │           │
				▼           ▼           ▼
		   SourceA    SourceB    Unknown
		   Mapper     Mapper     Source?
			 │          │          │
			 │          │          ▼
			 │          │      Default to
			 │          │      "Direct"
			 │          │      (deserialize
			 │          │       directly)
			 │          │
			 └────┬─────┘
				  │
		Mapper returned
		(or null if not found)
```

## 3. Adding a New Source

```
Existing Architecture          New Source Addition
───────────────────────────────────────────────────

MapperFactory ◄────────────────────── MapperFactory
	│                                      │
	├─ SourceA                             ├─ SourceA
	├─ SourceB                             ├─ SourceB
	│                                      ├─ SourceC  ◄── NEW
	│                                      │
	▼                                      ▼

					NEW MAPPER CREATED:

					SourceCMessage
					SourceCMapper
					Implements IMessageMapper<object>

					Registered in:
					1. MessageSources.cs (constant)
					2. MapperFactory.InitializeMappers()
					3. Service Bus message property
```

## 4. Design Patterns Visualization

### Strategy Pattern
```
	IMessageMapper<T> (Abstract Strategy)
		   △
		   │ implements
		   │
	┌──────┼──────┬────────┐
	│      │      │        │
	▼      ▼      ▼        ▼
 SourceA SourceB Direct  SourceC
 Mapper  Mapper  Mapper  Mapper...

Each mapper implements different
transformation strategy for its source
```

### Factory Pattern
```
	Message Processor (Client)
			 │
			 │ requests mapper
			 │
			 ▼
	┌─────────────────────┐
	│  MapperFactory      │
	│  (Encapsulation)    │
	│                     │
	│  - Maintains        │
	│    registry         │
	│  - Selects mapper   │
	│  - Returns mapper   │
	└──────────┬──────────┘
			   │
	  ┌────────┼────────┐
	  │        │        │
	  ▼        ▼        ▼
	SourceA  SourceB  Direct
	Mapper   Mapper   Mapper

Factory abstracts creation and
selection of appropriate mapper
```

### Dependency Injection
```
Program.cs:
  services.AddMessageMappers()
	│
	├─ Register SourceAMapper
	├─ Register SourceBMapper
	├─ Register MapperFactory
	└─ Register all in DI Container

At Runtime:

  Anywhere in code:
	public Service(IMapperFactory factory)
	{
		_factory = factory;
	}

  DI Container provides:
	SourceAMapper instance
	SourceBMapper instance
	MapperFactory instance
	All dependencies resolved!
```

## 5. Message Flow Timeline

```
Timeline of Message Processing
──────────────────────────────

T=0ms   ├─ Message arrives from Service Bus
		│
T=1ms   ├─ AzureServiceBusConsumerService
		│  - Logs message reception
		│  - Calls ProcessMessageAsync()
		│
T=2ms   ├─ ExtractMessageSource()
		│  - Reads ApplicationProperties["Source"]
		│  - Finds: "SourceA"
		│
T=3ms   ├─ DeserializeAndMapMessage()
		│  - Calls mapperFactory.MapMessage("SourceA", raw)
		│
T=4ms   ├─ MapperFactory.GetMapper("SourceA")
		│  - Dictionary lookup: O(1)
		│  - Returns: SourceAMapper instance
		│
T=5ms   ├─ SourceAMapper.Map(rawMessage)
		│  - Validates required fields
		│  - Transforms field names
		│  - Returns: CompanyInputDto
		│
T=6ms   ├─ CompanyMessageProcessor.ProcessCompanyAsync()
		│  - Validates DTO
		│  - Calls handler
		│
T=7ms   ├─ AddCompanyEventHandler.HandleAsync()
		│  - Checks for duplicates
		│  - Creates Company entity
		│  - Saves to database
		│
T=8ms   ├─ Transaction committed
		│
T=9ms   ├─ Message completed in Service Bus
		│  - Message won't reprocess
		│
T=10ms  └─ Complete!

Total Processing Time: ~10ms
```

---

## Key Takeaways

1. **Separation of Concerns**: Each component has a single responsibility
2. **Extensibility**: New sources added without modifying existing code
3. **Testability**: Each mapper can be tested independently
4. **Maintainability**: Clear, understandable flow
5. **Performance**: O(1) mapper selection, singleton instances
6. **Reliability**: Comprehensive error handling and logging
