# Nulah.HospitalHelper

## Assumptions

- The helper is a functional prototype for demonstration purposes with database source unknown
- API first design
- Authorisation source unknown, so basic authentication is used to demonstrate rudimentary access control
- With functional prototypes quickly becoming production prototypes, appropriate design measures have been done from the outset to ensure functionality is tested, and that database/authentication can be swapped out to the appropriate production sources
	- In saying that, user authentication can be a complex beast depending on services, environments, and access controls required, so has been left purposefully difficult to swap out to ensure at least some resistance is encountered to create some barrier that will require someone to sign off on it...
- Patient URN is a 7 -character unique string, sample data shows a 0 padded number, so URN will be stored as an auto incrementing int and 0 padded to a length of 7 when displayed to the user
	- Additionally only one URN is given, and 7 is a very low max length for an identification system, so for future functionality it will be assumed to be an int so the upper bound of URN's isn't constrained to just above the count of people in the most populated state in Australia.
	- The display format will be configurable however
- Patients are assumed to be from different cultural backgrounds, and may not have a concept of a first or last name, so patients will have a full name which is the entriety of the sum parts of their name, and then a display name will be used, with display first names and display last names to normalise to an expected format
	- A display name may simply be their display first name, and a display last name is not guaranteed to be non-null
- Employees are assumed to have no publically visible Id, but will use an auto-incrementing int to maintain consistency in API calls
	- Employee naming convention follows the same as Patients

## Running

- From Visual Studio:
	- The solution is configured to start both the API and front ends respectively once start is clicked.

## Testing

- Default seeded database will have multiple employees with the password: `Bas1c_P@ssw0rd`
- To start, the first employee has a username of `Pascal N.`