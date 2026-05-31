# 🎯 Local OpenTelemetry Development - Start Here

Welcome! Your local OpenTelemetry observability environment is ready.

## ⚡ Quick Start (3 minutes)

```powershell
# 1. Start the OTEL stack
cd docker-compose
.\startup.ps1

# 2. Start your app (in new terminal)
dotnet run --project src/MyStartUpCompany.Api

# 3. Generate traffic (in new terminal)
Invoke-WebRequest http://localhost:5000/api/companies

# 4. View results
# Jaeger:     http://localhost:16686
# Prometheus: http://localhost:9090
# Grafana:    http://localhost:3000 (admin/admin)
```

Done! ✅

---

## 📚 Documentation (Pick One)

### 🎯 I want to... | Read This
- **Get started quickly** → [OTEL_QUICK_REFERENCE.md](OTEL_QUICK_REFERENCE.md)
- **Understand how to test OTEL** → [LOCAL_OTEL_TESTING_GUIDE.md](docs/LOCAL_OTEL_TESTING_GUIDE.md)
- **Learn the architecture** → [OPENTELEMETRY_ARCHITECTURE.md](docs/OPENTELEMETRY_ARCHITECTURE.md)
- **Do common tasks** → [OBSERVABILITY_RUNBOOK.md](docs/OBSERVABILITY_RUNBOOK.md)
- **Validate OTEL works** → [OTEL_CONFIDENCE_CHECKLIST.md](docs/OTEL_CONFIDENCE_CHECKLIST.md)
- **Understand what was built** → [OTEL_IMPLEMENTATION_SUMMARY.md](OTEL_IMPLEMENTATION_SUMMARY.md)
- **See all documentation** → [docs/README.md](docs/README.md)

---

## ✅ What's Ready

### Local Observability Stack
- ✅ **Jaeger** - View distributed traces
- ✅ **Prometheus** - Query metrics
- ✅ **Grafana** - Create dashboards

### Applications (Instrumented)
- ✅ **API** - REST service with OTEL
- ✅ **Worker** - Background jobs with OTEL
- ✅ **Notifier** - Notifications with OTEL

### Testing & Validation
- ✅ **29 automated tests** - OTEL functionality verified
- ✅ **Health check script** - Validate local setup
- ✅ **Confidence checklist** - Complete validation guide

---

## 🚀 Today's Tasks

### For Individual Contributors
1. **Get oriented** (5 min)
   ```powershell
   cd docker-compose && .\startup.ps1
   ```

2. **View your first trace** (5 min)
   - Start API: `dotnet run --project src/MyStartUpCompany.Api`
   - Make request: `Invoke-WebRequest http://localhost:5000/api/companies`
   - Open Jaeger: http://localhost:16686
   - Click on trace to see spans

3. **Check metrics** (5 min)
   - Open Prometheus: http://localhost:9090
   - Query: `http_requests_total`
   - See what happened

4. **Read the guide** (10 min)
   - Read: [OTEL_QUICK_REFERENCE.md](OTEL_QUICK_REFERENCE.md)

### For Team Leads
1. **Understand scope** (10 min)
   - Local-only for now (Azure deferred)
   - See: [SCOPE_REVIEW.md](SCOPE_REVIEW.md)

2. **Validate setup** (5 min)
   - Run: `.\validate-otel-local.ps1`
   - See all services green

3. **Plan team onboarding** (15 min)
   - Read: [OTEL_IMPLEMENTATION_SUMMARY.md](OTEL_IMPLEMENTATION_SUMMARY.md)
   - Prepare team training

---

## 🎯 What is OpenTelemetry?

**Observability** through three signal types:

| Signal | Tool | What It Shows |
|--------|------|----------------|
| **Traces** | Jaeger | Request flow, latency, service calls |
| **Metrics** | Prometheus | Performance counters, rates, durations |
| **Logs** | Console | Events, errors, correlation IDs |

All three are **automatically collected** from your .NET applications.

---

## 📊 Access Your Observability Tools

```
┌─────────────────────────────────┐
│  OBSERVABILITY TOOLS            │
├─────────────────────────────────┤
│ Jaeger:     http://localhost:16686
│ Prometheus: http://localhost:9090
│ Grafana:    http://localhost:3000 (admin/admin)
└─────────────────────────────────┘
```

**All running locally** in Docker containers.

---

## ✨ Key Features

✅ **Zero Setup** - Docker Compose does all the work  
✅ **Real-Time** - See traces/metrics as requests happen  
✅ **Three Signals** - Traces, metrics, and logs all captured  
✅ **Production-Ready** - Same tools used in production  
✅ **Free** - Jaeger, Prometheus, Grafana all open source  
✅ **Documented** - Complete guides for every task  

---

## 🆘 Something Not Working?

### "Stack won't start"
```powershell
docker-compose down -v
cd docker-compose && .\startup.ps1
```

### "No traces in Jaeger"
1. Check app console for errors
2. Verify http://localhost:4318 endpoint
3. Restart app and regenerate traffic

### "No metrics in Prometheus"
1. Check http://localhost:9090/targets
2. Verify metrics endpoint: http://localhost:9091/metrics
3. Check appsettings.Development.json config

**More help**: → [OBSERVABILITY_RUNBOOK.md](docs/OBSERVABILITY_RUNBOOK.md)

---

## 📖 Reading Order (Recommended)

1. ✅ **This file** (you are here) - 3 min
2. 📖 **[OTEL_QUICK_REFERENCE.md](OTEL_QUICK_REFERENCE.md)** - Keep handy - 5 min
3. 📘 **[LOCAL_OTEL_TESTING_GUIDE.md](docs/LOCAL_OTEL_TESTING_GUIDE.md)** - Full setup - 10 min
4. 📚 **[OBSERVABILITY_RUNBOOK.md](docs/OBSERVABILITY_RUNBOOK.md)** - Common tasks - 15 min
5. 🎓 **[OPENTELEMETRY_ARCHITECTURE.md](docs/OPENTELEMETRY_ARCHITECTURE.md)** - Deep dive - 20 min

**Total**: ~60 minutes to expert

---

## 🎯 Success Metrics

When you've succeeded:

- [ ] `.\validate-otel-local.ps1` shows all green
- [ ] You can see traces in Jaeger
- [ ] You can query metrics in Prometheus
- [ ] You created a Grafana dashboard
- [ ] You ran and understood the tests
- [ ] You can explain the three signals

---

## 🚀 Let's Begin!

Ready to see your application being observed in real-time?

```powershell
# Start here:
cd docker-compose
.\startup.ps1

# Then in another terminal:
dotnet run --project src/MyStartUpCompany.Api

# Then:
Invoke-WebRequest http://localhost:5000/api/companies

# Finally:
# Open http://localhost:16686 to see the trace!
```

---

## 📞 Need Help?

| Question | Answer |
|----------|--------|
| How do I start the stack? | [OTEL_QUICK_REFERENCE.md](OTEL_QUICK_REFERENCE.md) - Top section |
| How do I fix X problem? | [OBSERVABILITY_RUNBOOK.md](docs/OBSERVABILITY_RUNBOOK.md) - Troubleshooting |
| How does OTEL work? | [OPENTELEMETRY_ARCHITECTURE.md](docs/OPENTELEMETRY_ARCHITECTURE.md) |
| How do I validate OTEL? | [OTEL_CONFIDENCE_CHECKLIST.md](docs/OTEL_CONFIDENCE_CHECKLIST.md) |
| Where's the full setup? | [LOCAL_OTEL_TESTING_GUIDE.md](docs/LOCAL_OTEL_TESTING_GUIDE.md) |

---

## 🎓 Key Concepts (30 seconds)

**Observability** = Understanding what's happening inside your system
- **Traces** show the path requests take through services
- **Metrics** show performance numbers over time
- **Logs** show specific events and errors

**Your setup** automatically collects all three from your .NET code.

---

## 📋 Scope

| Scope | Status | Note |
|-------|--------|------|
| Local OTEL setup | ✅ Complete | Ready to use today |
| Local testing | ✅ Complete | Full validation guide |
| Azure deployment | 📦 Deferred | Ready when you are |
| K8s manifests | 📦 Deferred | In k8s_archived/ |

---

## 🎯 Next Steps

1. **Right now**: Follow the quick start above (3 min)
2. **Next**: Read OTEL_QUICK_REFERENCE.md (5 min)
3. **Today**: Complete LOCAL_OTEL_TESTING_GUIDE.md (10 min)
4. **This week**: Team training on local observability

---

**Status**: ✅ Ready for local development  
**Scope**: Local development & testing  
**Next**: When ready for cloud, see docs_archived/  

Let's observe! 🎯
