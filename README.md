# Polly Demo

This is a demo solution which contains 2 ASP.NET core web APIs. The first one sends requests and the other sends responses. With these 2 APIs different policy types of Polly are demonstrated. 

•	Polly is de-facto(standard) resilience and & transient fault handling library for .NET. 

•	With this we can create retry policies in .NET apps.

•	Polly Policy types

## 1.	Retry Policy
•	Can use exponential backoff to gradually increase wait time between retries.
•	Retry for 5 times, in between take 2s, 4s, 9s, 16 seconds … breaks

## 2.	Circuit breaker
•	Retry 10 times, then break out and take 10 seconds test, retry for another 10 times …

## 3.	Timeout
•	If the response is taking more than 5 seconds, then cancel request (do not wait more than 5 s), throw time out exception, handle it from controller, and send appropriate error response to client of API

## 4.	Bulkhead isolation
•	Allows to limit the number of concurrent requests to prevent resource exhaustion. 
•	Prevents DDOS attacks 
•	Example- Allow 5 concurrent requests, then queue another 10, then reject all others 

## 5.	Fall back
•	Provides default response when fails so that client gets a graceful error message rather than surprise application failures 
