# Arrowhead Client Library C#
A client library for creating Service Providers and Consumers for [Arrowhead](https://www.arrowhead.eu) written in C#.
The intention of this library is to make it eaiser for developers to register and orchestrate services by providing an interface against the [Core Systems](https://github.com/arrowhead-f/core-java-spring).

An example of how to use the library with a basic Producer and Consumer can be in the example folder in this repo.

## .NET versions Dependencies
* .NET Core 5
* .NET Framework 4.8

## Runtime Dependencies
To run a program with the library the mandatory core Arrowhead systems must be running and their ip addresses and ports must be configured correctly.

Client certificates must also be provided, a guide on how to do this can be found [here](https://github.com/arrowhead-f/core-java-spring/blob/master/documentation/certificates/create_client_certificate.pdf).

For use of the admin functionality in the Arrowhead C# library a sysop certificate from the local cloud is needed. These admin parts are used for adding Authorization Intracloud rules and Orchestration store entries which can also be done via the [Arrowhead Management Tools](https://github.com/arrowhead-tools/mgmt-tool-js), requests to the API via Swagger or queries directly to the local SQL database.


## Limitations
This project is in a very early development state and thus it has some major limitations. This includes but is not limited to:
* This library only connects to the three mandatory core systems
* No existing test suite
* Lacking error handling
