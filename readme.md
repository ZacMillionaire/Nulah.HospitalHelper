# Nulah.HospitalHelper

## Assumptions

- The helper is a functional prototype for demonstration purposes with database source unknown
- API first design
- Authorisation source unknown, so basic authentication is used to demonstrate rudimentary access control
- With functional prototypes quickly becoming production prototypes, appropriate design measures have been done from the outset to ensure functionality is tested, and that database/authentication can be swapped out to the appropriate production sources
	- In saying that, user authentication can be a complex beast depending on services, environments, and access controls required, so has been left purposefully difficult to swap out to ensure at least some resistance is encountered to create some barrier that will require someone to sign off on it...
- Patient URN is a 7-character unique string, sample data shows a 0 padded number, so URN will be stored as an auto incrementing int and 0 padded to a length of 7 when displayed to the user
	- Additionally only one URN is given, and 7 is a very low max length for an identification system, so for future functionality it will be assumed to be an int so the upper bound of URN's isn't constrained to just above the count of people in the most populated state in Australia.
	- The display format will be configurable however
- Patients are assumed to be from different cultural backgrounds, and may not have a concept of a first or last name, so patients will have a full name which is the entriety of the sum parts of their name, and then a display name will be used, with display first names and display last names to normalise to an expected format
	- A display name may simply be their display first name, and a display last name is not guaranteed to be non-null
- Employees are assumed to have no publically visible Id, but will use an auto-incrementing int to maintain consistency in API calls
	- Employee naming convention follows the same as Patients
- Patient comments are related to patients, so will not have their own repository to manage them and are instead handled by the PatientManager
- Test run length isn't a consideration, but due to reseeding the database for each test, test duration may be unreasonably long depending on processor
	- Ideally I'd just be doing an in-memory mock of the repository but I decided to double dip on test data to speed up also creating the database and repositories
- Admitting a patient (assigning to a bed) requires a presenting issue
- Creating an employee has no uniqueness constraints as names are not a unique identifier despite best efforts in the world.
	- Unless your birth name is a Guid in which case uh
	- Basically multiple employees can exist with the same first, last and display names, just like in real life
- Creating employees, beds and patients aren't specified, but it'd be odd for them to not exist, so those features have been added.
- Not being logged in will redirect all requests to a login page, and unauthenticated API requests will be denied
- Data and API models are separated to prevent "accidental" data creation, API models are prefixed with 'Public' by convention
	- The only exception are Enums which can be left as is
- The front end has a hard dependency on the API project due to time, however in a realistic scenario with more time, the projects are designed so that the API project can run stand alone, and the front end can similarly do the same, were it designed with a SPA framework
	- Regardless of this, the API project _can_ run stand alone, and will present a swagger UI when run in a development environment.

## Out of Scope

- Role based authentication
- A safe and secure authentication system
- Permission management
	- All employees can admit, discharge, create beds, patients or new employees

## Limitations

- Should this extend past a prototype stage, the methods used for getting the list of beds with admitted patients would need to be revised, as presently the current process retrieves _all_ comments for a patient if a bed is not in use. While this may be trivial to start, as comments are added/patients are added/both, the amount of data retrieved will increase and eventually lead to performance issues.
	- This can easily be resolved by modifying the query to return only the latest comment with an optional `boolean` overload


## Running

- From Visual Studio:
	- The solution is configured to start both the API and front ends respectively once start is clicked.

## Testing

- Default seeded database will have multiple employees with the password: `Bas1c_P@ssw0rd`
- To start, the first employee has a username of `Pascal N.`