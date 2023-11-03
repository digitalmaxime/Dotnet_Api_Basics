# Dotnet_Basics

A good (basic) Api should have : 
- Versioning (api/v1/person)
- Validation
- OpenApi Documentation
- Contracts
- Solid principle (use Mediatr)
- 4 layers (Api, Application, Infra, Domain)
- Respect naming conventions (endpoint not using verbs, sql table name SINGULAR)
- Use configurations (IOptions pattern)
- Use Logging
- Basic error handling
- Use of PUT vs PATCH methods (must be Idempotent)

The WebApi has 2 fondamentals components: 
- The WebApplicationBuilder
  - Where all Api configs live
- The Web Application itself
  - Where all middlewares are configured

ref : https://www.youtube.com/playlist?list=PL59L9XrzUa-nqfCHIKazYMFRKapPNI4sP
