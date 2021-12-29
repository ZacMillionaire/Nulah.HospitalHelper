# Nulah.HospitalHelper

## Assumptions

- The helper is a functional prototype for demonstration purposes with database source unknown
- API first design
- Authorisation source unknown, so basic authentication is used to demonstrate rudimentary access control
- With functional prototypes quickly becoming production prototypes, appropriate design measures have been done from the outset to ensure functionality is tested, and that database/authentication can be swapped out to the appropriate production sources
	- In saying that, user authentication can be a complex beast depending on services, environments, and access controls required, so has been left purposefully difficult to swap out to ensure at least some resistance is encountered to create some barrier that will require someone to sign off on it...

## Running

- From Visual Studio:
	- The solution is configured to start both the API and front ends respectively once start is clicked.

## Testing

- Default seeded database will have multiple employees with the password: `Bas1c_P@ssw0rd`
- To start, the first employee has a username of `Pascal N.`