### Test Description
In the 'PaymentService.cs' file you will find a method for making a payment. At a high level the steps for making a payment are:

 - Lookup the account the payment is being made from
 - Check the account is in a valid state to make the payment
 - Deduct the payment amount from the account's balance and update the account in the database
 
What we’d like you to do is refactor the code with the following things in mind:  
 - Adherence to SOLID principals
 - Testability  
 - Readability 

We’d also like you to add some unit tests to the ClearBank.DeveloperTest.Tests project to show how you would test the code that you’ve produced. The only specific ‘rules’ are:  

 - The solution should build.
 - The tests should all pass.
 - You should not change the method signature of the MakePayment method.

You are free to use any frameworks/NuGet packages that you see fit.  
 
You should plan to spend around 1 to 3 hours to complete the exercise.

---

## Refactor Notes

### What I changed

- Changed MakePaymentResult to a record type: this gives cleaner value semantics and reveal intention as a result object
- Created factory class to create account data store: this centralises data-store selection logic and reduces branching
- Added IOptions for configuration: this removes static config coupling and makes testing easier
- Inject an `IAccountDataStoreFactory` into the PaymentService for testing: Allows for DI and enables unit tests
- Extract payment validation to its own function and added switch expression: isolates rule logic and improves readability/maintainability
- Added logs to `AccountDataStoreFactory` and `PaymentService`: improves observability and makes production troubleshooting/audit trails easier
- Enable nullable and implicit using in csproj: improves baseline safety and reduces boilerplate noise
- Added unit tests with NUnit and FakeItEasy: validates rule behavior quickly and supports safe refactoring
- Added [Flags] attribute to make it explicit that the enum is being use as a bit field

### What I could change
- Idempotency key/outbox pattern: protects against duplicate processing and improves reliability in distributed/evented flows
- Atomic payment update: there's a chance balance could change between getting account, validation and update, perform in one DB transaction
- Write immutable debit entry to a ledger: balance is a read model
- Dockerfile: for easier local dev and deployment
- Integration and automated tests if appropriate: verifies full component/system test
- Assert on log calls if they're for audit purposes or ops critical
- Directory.Build.Props, Directory.Packages.props, .editorconfig: set project wide standards, enforce coding standards, centralise package management
- Add correlation id to logs (Open Telemetry, ELK stack etc): makes end-to-end tracing a lot faster during live incidents
- Fluent validation / Individual validator classes: for payment schemes if there were more / complex validation rules
- If I was super strict on the liskov substitution pattern I would have used a couple adapater classes rather than make `AccountDataStore` and `BackupAccountDataStore` and inherit from `IAccountDataStore`, would also mean the 2 classes would be unchanged
- Error handling, assumption is that there would be a global error handler of some sort and/or try/catch nearer entry point of work flow
- Add more directories if project grows in size, for this exercise minimal is better
- Pass canncellation token through from entry point
- Add async I/O if needed