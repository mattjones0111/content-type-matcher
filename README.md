# Content Type Matching

This repository shows an implementation of MatcherPolicy for asp.net core that discriminates 
endpoints based on the `Content-Type` defined in the HTTP header, making it then possible to 
have two endpoints with identical URLs and HTTP-methods, but to be differentiated by content 
type.
