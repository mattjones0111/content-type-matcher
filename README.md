# Content Type Matching

https://github.com/microsoft/aspnet-api-versioning provides extension methods and types that 
allow endpoints within an Asp.Net Core web host to be selected based on version numbers provided
either through headers or through the URL route. It acknowledges however that versioning by
custom media types is not supported.

This repository shows an implementation of MatcherPolicy for Asp.Net Core that discriminates 
endpoints based on the `Content-Type` defined in the HTTP header and thus custom content types,
making it then possible to have two endpoints with identical URLs and HTTP-methods, but to be
differentiated by content type.

This is a work in progress.
