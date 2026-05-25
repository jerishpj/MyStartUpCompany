# Visual Architecture Guide

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                       AZURE SERVICE BUS TOPIC                            │
│  Messages from: SourceA, SourceB, SourceC, Direct (normalized messages) │
└───────────────────────────────┬─────────────────────────────────────────┘
								│
								│ Service Bus Subscription
								│
				┌───────────────▼────────────────┐
				│ AzureServiceBusConsumerService │
				│  - Receives messages           │
				│  - Extracts source metadata    │
				│  - Deserializes payload        │
				└───────────────┬────────────────┘
								│
								│ ServiceBusReceivedMessage
								│
				  ┌─────────────▼──────────────┐
				  │  ExtractMessageSource()    │
				  │  Looks for:                │
				  │  1. ApplicationProperties  │
				  │  2. Subject property       │
				  │  3. Default: "Direct"      │
				  └─────────────┬──────────────┘
								│
				┌───────────────▼────────────────┐
				│   MapperFactory.MapMessage()   │
				│  Uses source to resolve mapper │
				└───────────────┬────────────────┘
								│
		┌───────────────────────┼───────────────────────┐
		│                       │                       │
		▼                       ▼                       ▼
  ┌─────────────┐        ┌─────────────┐        ┌─────────────┐
  │ SourceAMap  │        │ SourceBMap  │        │ SourceCMap  │
  │    per      │        │    per      │        │    per      │
  │  Strategy   │        │  Strategy   │        │  Strategy   │
  │  Pattern    │        │  Pattern    │        │  Pattern    │
  └──────┬──────┘        └──────┬──────┘        └──────┬──────┘
		 │                      │                      │
		 │ CompanyName          │ OrganizationName     │ BizName
		 │ → Name               │ → Name               │ → Name
		 │ StreetAddress        │ MainLocationAddr     │ BizAddress
		 │ → Address            │ → Address            │ → Address
		 │ CityName             │ CityLocation         │ BizCity
		 │ → City               │ → City               │ → City
		 │ (etc...)             │ (etc...)             │ (etc...)
		 │                      │                      │
		 └───────────────────────┼───────────────────────┘
								 │
					┌────────────▼──────────────┐
					│  CompanyInputDto (Norm)   │
					│  - Name                   │
					│  - Address                │
					│  - City                   │
					│  - Region                 │
					│  - PostalCode             │
					│  - Country                │
					│  - Phone                  │
					│  - Description (optional) │
					└────────────┬──────────────┘
								 │
				  ┌──────────────▼──────────────┐
				  │ CompanyMessageProcessor     │
				  │  - Validates data          │
				  │  - Checks for duplicates   │
				  │  - Calls event handler     │
				  └────────────┬─────────────────┘
							   │
					┌──────────▼──────────┐
					│ AddCompanyEventHand │
					│ ler (no changes)    │
					│ - Persists to DB    │
					│ - Returns result    │
					└──────────┬──────────┘
							   │
					┌──────────▼──────────┐
					│   SQL Database      │
					│ Company Table       │
					│ (fixed schema)      │
					└─────────────────────┘
```

## Mapper Resolution Flowchart

```
Message Arrives
	│
	▼
Extract Source from Message Metadata
	│
	├─► ApplicationProperties['Source']? ──► YES ──┐
	│                                              │
	├─► Subject property? ──────────────► YES ──┤ Source Identifier
	│                                           │
	└─► Use "Direct" (default) ────────────────┘
												 │
												 ▼
							MapperFactory.GetMapper(source)
												 │
					┌────────────┬────────────┬──┴─────────┐
					│            │            │            │
					▼            ▼            ▼            ▼
			SourceA?  SourceB?  SourceC?   Unknown?
					│            │            │            │
					▼            ▼            ▼            ▼
				SourceA      SourceB      SourceC       null/log
				Mapper       Mapper       Mapper       warning
					│            │            │
					└────────────┴────────────┘
								 │
								 ▼
						Mapper.Map(message)
								 │
						┌────────┴────────┐
						│                 │
						▼                 ▼
					Success:          Failure:
					Return DTO        Return null
						│                 │
						├─────────┬───────┘
								  │
								  ▼
					  CompanyMessageProcessor
								  │
						┌─────────┴─────────┐
						│                   │
						▼                   ▼
				  Continue:           Stop & Log:
				  Process DTO         Error + skip
```

## Dependency Injection Wiring

```
Program.cs (Composition Root)
	│
	├─► builder.Services.AddLogging()
	├─► builder.Services.AddDatabase()
	├─► builder.Services.AddMessageMappers() ◄─── New extension method
	│       │
	│       ├─► services.AddSingleton<SourceAMapper>()
	│       ├─► services.AddSingleton<SourceBMapper>()
	│       ├─► services.AddSingleton<SourceCMapper>()
	│       └─► services.AddSingleton<IMapperFactory, MapperFactory>()
	│               │
	│               └─► MapperFactory constructor:
	│                       ├─► Get SourceAMapper from ServiceProvider
	│                       ├─► Get SourceBMapper from ServiceProvider
	│                       ├─► Get SourceCMapper from ServiceProvider
	│                       └─► Register in dictionary by source ID
	│
	├─► builder.Services.AddScoped<CompanyMessageProcessor>()
	│       │
	│       └─► ctor(IMapperFactory mapperFactory)
	│           // Uses injected factory for mapping
	│
	├─► builder.Services.AddHostedService<AzureServiceBusConsumerService>()
			│
			└─► ctor(IMapperFactory, ICompanyMessageProcessor)
				// Uses injected factory + processor

Service Injection at Runtime:
	│
	├─► AzureServiceBusConsumerService receives message
	│   ├─► Extracts source metadata
	│   ├─► Calls _mapperFactory.MapMessage(source, rawMessage)
	│   │   └─► Factory resolves correct mapper
	│   │       └─► Mapper transforms to DTO
	│   └─► Passes DTO to _companyMessageProcessor.ProcessCompanyAsync()
```

## Source Addition Workflow (Adding Source D)

```
Step 1: Create DTO Class
		├─► File: SourceDMessage.cs
		├─► Properties: Source-specific field names
		└─► No dependencies

Step 2: Create Mapper
		├─► File: SourceDMapper.cs
		├─► Implements: IMessageMapper<object>
		├─► Property Mapping Logic
		├─► Validation
		└─► Error Logging

Step 3: Register Source Constant
		├─► File: MessageSources.cs
		├─► Add: const string SourceD = "SourceD"
		├─► Add: SourceD to enum
		└─► Update: FromIdentifier(), ToIdentifier(), GetAllSources()

Step 4: Register in DI
		├─► File: MapperExtensions.cs
		├─► Add: services.AddSingleton<SourceDMapper>()
		└─► No other changes needed

Step 5: Register in Factory
		├─► File: MapperFactory.cs
		├─► Add: Mapper resolution in InitializeMappers()
		├─► Try-catch registration
		└─► Logging

Build & Test
		├─► Solution compiles
		└─► All tests pass

✅ Complete - Zero impact on core logic!
```

## Data Transformation Example

```
SOURCE A FORMAT (from Topic)
──────────────────────────
{
  "CompanyName": "Tech Solutions Inc",
  "CompanyDescription": "Cloud services provider",
  "StreetAddress": "123 Innovation Drive",
  "CityName": "San Francisco",
  "StateCode": "CA",
  "ZipCode": "94103",
  "CountryCode": "USA",
  "PhoneNumber": "555-0100"
}
		│ SourceAMapper.Map()
		▼
DATABASE SCHEMA (normalized)
───────────────────────────
{
  "Name": "Tech Solutions Inc",
  "Description": "Cloud services provider",
  "Address": "123 Innovation Drive",
  "City": "San Francisco",
  "Region": "CA",
  "PostalCode": "94103",
  "Country": "USA",
  "Phone": "555-0100"
}


SOURCE B FORMAT (from Topic)
──────────────────────────
{
  "OrganizationName": "Enterprise Systems Ltd",
  "OrganizationDesc": "ERP solutions",
  "MainLocationAddr": "456 Corporate Blvd",
  "CityLocation": "New York",
  "StateRegion": "NY",
  "PostalCode": "10001",
  "NationCode": "USA",
  "ContactPhone": "555-0200"
}
		│ SourceBMapper.Map()
		▼
DATABASE SCHEMA (normalized)
───────────────────────────
{
  "Name": "Enterprise Systems Ltd",
  "Description": "ERP solutions",
  "Address": "456 Corporate Blvd",
  "City": "New York",
  "Region": "NY",
  "PostalCode": "10001",
  "Country": "USA",
  "Phone": "555-0200"
}


SOURCE C FORMAT (from Topic)
──────────────────────────
{
  "BizName": "Digital Innovations Co",
  "BizDescription": "Software development",
  "BizAddress": "789 Tech Street",
  "BizCity": "Austin",
  "BizProvince": "TX",
  "BizZip": "78701",
  "BizCountry": "USA",
  "BizPhone": "555-0300"
}
		│ SourceCMapper.Map()
		▼
DATABASE SCHEMA (normalized)
───────────────────────────
{
  "Name": "Digital Innovations Co",
  "Description": "Software development",
  "Address": "789 Tech Street",
  "City": "Austin",
  "Region": "TX",
  "PostalCode": "78701",
  "Country": "USA",
  "Phone": "555-0300"
}

All three sources map to the same database schema
with zero changes to persistence logic!
```

## Error Handling Strategy

```
Mapper Invocation
		│
		▼
Try to deserialize message
		│
	┌───┴───┐
	│       │
Success   Error
	│       │
	▼       ▼
Validate  Log Warning
Required  "Deserialization
Fields    failed for..."
	│       │
	├─────┬─┘
	│     │
Success Fail
	│     │
	▼     ▼
  Map   Return null
 Fields (graceful
	│   degradation)
	├─────┬──┐
	│     │  │
Try  Trim Handle
 Map Props Exceptions
	│     │  │
	│     │  ▼
	│     │  Log Error
	│     │  "Error mapping
	│     │   Source X:..."
	│     │  │
	└─────┴──┴─► Return DTO or null
					 │
					 ▼
			CompanyMessageProcessor
					 │
			┌────────┴────────┐
			│                 │
		  null            CompanyInputDto
			│                 │
			▼                 ▼
		Skip & Log       Validate &
		"Failed to       Persist
		map message"
```

## Factory Registry Dictionary

```
MapperFactory._mappers Dictionary
┌────────────────────────────────────────────┐
│ Key (Source)  │  Value (Mapper Instance)   │
├───────────────┼────────────────────────────┤
│ "SourceA"     │ → SourceAMapper instance   │
│ "SourceB"     │ → SourceBMapper instance   │
│ "SourceC"     │ → SourceCMapper instance   │
│ "SourceD"     │ → SourceDMapper instance   │ (when added)
│ "SourceE"     │ → SourceEMapper instance   │ (when added)
└────────────────────────────────────────────┘

Lookup: O(1) - Direct dictionary key access
(Case-insensitive due to StringComparer.OrdinalIgnoreCase)
```

## Component Interactions Timeline

```
Time    AzureServiceBus    Consumer            Processor           Database
 │      │                  │                   │                   │
 ├─────►│ Message arrives  │                   │                   │
 │      │ (with source     │                   │                   │
 │      │  metadata)       │                   │                   │
 │      │                  │                   │                   │
 ├──────┼─────────────────►│ Receive message   │                   │
 │      │                  │ Extract source ID │                   │
 │      │                  │ Deserialize       │                   │
 │      │                  │ Call MapFactory   │                   │
 │      │                  │                   │                   │
 │      │                  │ ┌─────────────┐   │                   │
 │      │                  │ │Look up map  │   │                   │
 │      │                  │ │per source   │   │                   │
 │      │                  │ │ID in dict   │   │                   │
 │      │                  │ └─────────────┘   │                   │
 │      │                  │ Call mapper.Map() │                   │
 │      │                  │                   │                   │
 │      │                  ├──────────────────►│ Validate DTO      │
 │      │                  │                   │ Check duplicates  │
 │      │                  │                   │ Call handler      │
 │      │                  │                   │                   │
 │      │                  │                   ├──────────────────►│ INSERT
 │      │                  │                   │                   │ Company
 │      │                  │                   │                   │
 │      │                  │                   │◄──────────────────┤ Success
 │      │                  │◄──────────────────┤ Return result     │
 │      │                  │ Complete message  │                   │
 │      │ ◄──────────────────────────────────┤ ACK               │
 │      │                  │                   │                   │
```

## Test Pyramid for Mapping Architecture

```
					Integration Tests
				   (Happy path flow)
					   ▲
					   │
					   │ (Fewer tests, 2-3%)
	┌──────────────────┼──────────────────┐
	│                  │                  │
	│    Mapper       │    Factory       │   Service Bus
	│   Unit Tests    │   Unit Tests     │   Consumer Tests
	│    (each)       │  (resolution)    │
	│                 │                  │
	└──────────────────┼──────────────────┘
					   │
					   │ (More tests, 7%)
					   ▼
				  Unit Tests
			  (Property mapping)
					 ▲
					 │
			  ┌──────┴──────┐
			  │             │
		  Source A     Source B
		  Mapper       Mapper
		  Tests        Tests
			 │             │
		  (15%)         (15%)
```

## Performance Characteristics

```
Message Processing Timeline

Message arrives at Topic ─────────────┐ T=0ms
									  │
Extract metadata & deserialize ───────┤ T=1-2ms
									  │
Look up mapper in factory dict ───────┤ T=0.1ms (O(1))
									  │
Call mapper.Map() & transform ────────┤ T=2-5ms (depends on
									  │         property count)
Validate normalized DTO ──────────────┤ T=0.5ms
									  │
Check for duplicates ─────────────────┤ T=2-10ms (DB query)
									  │
Insert to database ────────────────────┤ T=5-20ms (DB write)
									  │
Mark message complete ─────────────────┤ T=1ms
									  ▼
Total end-to-end: ~15-40ms per message
```

---

## Key Takeaways

1. **Strategy Pattern** → Each source has isolated mapper
2. **Factory Pattern** → Dynamic mapper resolution by source ID
3. **Dependency Injection** → Loose coupling, testability
4. **Graceful Degradation** → Null returns, not exceptions
5. **Centralized Constants** → No magic strings, type-safe
6. **O(1) Lookup** → Dictionary-based mapper registry
7. **Zero Core Logic Changes** → When adding new sources
8. **Backward Compatible** → Direct format still works
