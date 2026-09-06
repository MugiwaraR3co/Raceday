# RaceDay

## Project Overview

RaceDay is a race event management system designed to manage
organisers, participants, events, race categories, enrolments
and race results.

This repository contains the Part 1 design and database
documentation for the RaceDay system.

## Part 1 Deliverables

### Entity Relationship Diagram

The ERD describes the database entities, attributes, primary
keys, foreign keys and relationships.

File:

`docs/RaceDay_ERD.png`

Editable diagram:

`docs/RaceDay_ERD.drawio`

### Database

The SQL Server database implementation is provided in:

`docs/RaceDay_Database.sql`

The database contains the following entities:

- Roles
- Users
- Events
- Categories
- EventCategories
- Enrolments
- Results

### API Endpoint Design

The planned REST API endpoints are documented in:

`docs/RaceDay_API_Endpoint_Plan.md`

## Technology

- SQL Server
- SQL Server Management Studio
- Visual Studio Code
- Git
- GitHub
- GitHub Actions
- diagrams.net

## Database Relationships

The database uses primary and foreign keys to maintain
relationships between users, events, categories, enrolments
and results.

The many-to-many relationship between Events and Categories
is resolved through the EventCategories table.

## Project Structure

```text
RaceDay
│
├── README.md
│
├── docs
│   ├── RaceDay_ERD.drawio
│   ├── RaceDay_ERD.png
│   ├── RaceDay_Database.sql
│   └── RaceDay_API_Endpoint_Plan.md
│
├── Evidence
│
└── .github
    └── workflows
    ## System Overview

RaceDay is a web-based event management system designed for the South African
road running, walking, and cycling community.

The system allows Organisers to manage events, categories, participants,
enrolments and results, while Participants can browse events, enter events,
view their enrolments and track their results.
## User Roles

### Organiser

The Organiser can:
- Create events
- Edit events
- Delete events
- Manage event categories
- Capture participant results
- View event enrolments

### Participant

The Participant can:
- Create an account
- Browse events
- Enter events by selecting a category
- View their enrolments
- Track their personal results
## Part 1 Deliverables

Part 1 consists of three main planning and database deliverables:

1. Entity Relationship Diagram (ERD)
2. API Endpoint Plan
3. SQL Database Script

These documents are stored inside the `docs` folder.
## Repository Structure

- `.github/workflows/` - GitHub Actions workflow
- `docs/` - Part 1 planning and database documentation
- `Evidence/` - screenshots and supporting evidence
- `README.md` - project documentation
## ERD

The RaceDay ERD represents the relational database structure required by the
system. It identifies the entities, attributes, primary keys, foreign keys
and relationships required to support organisers, participants, events,
categories, enrolments and results.

The completed ERD is available in:

`docs/RaceDay_ERD.png`
