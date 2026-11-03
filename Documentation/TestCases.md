
# Expense Reimbursement Management System
## Test Case Documentation

**Project:** Expense Reimbursement Management System  
**Technology:** C#, .NET 10, ASP.NET Core MVC, Web API, SQL Server  
**Database:** ExpenseDB

## Test Results

| Test ID | Scenario | Expected Result | Actual Result | Status |
|---|---|---|---|---|
| TC01 | Submit a valid claim | Claim created with Pending status | A new claim was created and displayed in the application | Pass |
| TC02 | Submit an empty employee name | Validation error | Not yet verified | Pending |
| TC03 | Submit Employee ID as zero | Validation error | Not yet verified | Pending |
| TC04 | Submit a negative amount | Validation error | Not yet verified | Pending |
| TC05 | Submit amount as zero | Validation error | Not yet verified | Pending |
| TC06 | Submit a future expense date | Validation error | Not yet verified | Pending |
| TC07 | Submit an unsupported category | Validation error | Not yet verified | Pending |
| TC08 | Retrieve an existing claim | HTTP 200 with claim data | Existing claim details opened successfully in the MVC application | Pass |
| TC09 | Retrieve a nonexistent claim | HTTP 404 | The MVC page displayed a not-found message for a nonexistent claim | Pass |
| TC10 | Retrieve claims for an employee | Only that employee's claims returned | Not yet verified through the employee-specific API endpoint | Pending |
| TC11 | Approve a Pending claim | Status changes to Approved | Claim status updates were tested in the application | Pass |
| TC12 | Reject a Pending claim | Status changes to Rejected | Claim status updates were tested in the application | Pass |
| TC13 | Mark an Approved claim as Paid | Status changes to Paid | An Approved claim was changed to Paid | Pass |
| TC14 | Approve an already Approved claim | HTTP 400 | Not yet verified | Pending |
| TC15 | Pay a Rejected claim | HTTP 400 | Not yet verified | Pending |
| TC16 | Change the status of a Paid claim | HTTP 400 | Not yet verified | Pending |
| TC17 | Open the dashboard | Summary values match API data | Dashboard displayed summary cards and latest claims | Pass |
| TC18 | Open the Console Summary | Summary values match API data | Console displayed 10 claims: 3 Pending, 2 Approved, 2 Rejected and 3 Paid | Pass |
| TC19 | Run console with API stopped | Error handled gracefully | Not yet verified | Pending |
| TC20 | Verify records in SSMS | Stored values match API responses | Ten database records and summary totals were verified in SSMS | Pass |

## Additional MVC and Console Tests

| Test | Scenario | Result |
|---|---|---|
| UI01 | Search Expense List by Employee ID | Pass |
| UI02 | Filter claims by Pending status | Pass |
| UI03 | Reset Expense List filters | Pass |
| UI04 | Display pending claims in Console | Pass |
| UI05 | Search Console using an existing Claim ID | Pass |
| UI06 | Search Console using nonexistent Claim ID 99999 | Pass |
| UI07 | Exit Console application | Pass |

## Notes

- Pass indicates the behavior was observed during manual testing.
- Pending indicates the test still needs to be executed and verified.
- HTTP status codes should be verified directly through the API or Swagger where the assessment specifies them.
- Record additional test evidence and screenshots where required.
