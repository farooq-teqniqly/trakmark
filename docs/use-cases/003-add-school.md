# Use Case 001 - Add School

## Summary
User adds school

## Actor
Registered user

## Preconditions
1. Registered user is authenticated.
2. Registered user is member of Admin role.
3. Cities have been added. See 001-add-cities.
4. School Districts have been added. See 002-add-school-districts.
5. Roles have been seeded by system.
6. Sports have been seeded by the system.

## Main Sequence
1. User clicks Admin drop down menu in nav bar.
2. User selects Add Schools nav bar item.
3. System shows Add Schools form.
4. User enters one or more (max 100) schools:
    - Name  (required)
    - City (required, drop down)
    - State (required, drop down)
    - School district (required, drop down)
    - Sports (required, drop-down with checkbox for each sport, Track & Field checked by default)
    > Note: school data is validated as user enters data:<br/>
        - Name max 100 characters <br/>
5. If form data valid, Save button is enabled.
6. User clicks Save.
7. Server validates form data.
8. If server validation succeeds, schools are persisted.
9. Notification toast saying Save is successful.

## Alternative Sequences
4A1. Validarion fails. The system shows the error message to the user.
5A1. User clicks Cancel. System navigates back to Home page. No data is modified.
8A1. Server validation or save fails. The system shows the error message to the user. Notification toast saying Save failed.

## Persistence Notes
When a school is added:
- Database sets created date to the current UTC time.
- Database sets last updated date to NULL.
- Database auto assigns surrogate key.
    
## Business Rules
Schools are equal if:
    - Names are equal
    - City, State, School Districts are equal.
    - Duplicates not allowed.