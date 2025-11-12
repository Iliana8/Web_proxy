# Lab 2 – Etapa 1 (C#)

Server HTTP concurent (thread-safe) ce oferă operații GET/PUT/POST pentru angajați.
Răspunsurile pot fi în JSON sau XML.

## Build & Run
```bash
dotnet run
# serverul pornește pe http://localhost:8081
```

## Exemple CURL
```bash
curl "http://localhost:8081/employee?id=1"
curl "http://localhost:8081/employees?offset=0&limit=2"
curl -X PUT http://localhost:8081/employee -H "Content-Type: application/json"      -d "{"Id":5,"Name":"Eve","Department":"QA"}"
curl -X PUT http://localhost:8081/employee -H "Content-Type: application/xml"      -d "<Employee><Id>6</Id><Name>Frank</Name><Department>Finance</Department></Employee>"
```
