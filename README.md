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