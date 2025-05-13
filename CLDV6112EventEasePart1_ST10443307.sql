USE master;

IF EXISTS (SELECT * FROM sys.databases WHERE NAME = 'EventEase')
DROP DATABASE EventEase
CREATE DATABASE EventEase
USE EventEase;

--TABLE CREATION AND INSERTION FOR Venue
CREATE TABLE Venues(
	VENUEID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	VenueName VARCHAR(250) NOT NULL,
	VenueLocation VARCHAR(250) NOT NULL,
	VenueCapacity INT NOT NULL,
	ImageURL NVARCHAR(100) NOT NULL,
);

INSERT INTO Venues(VenueName, VenueLocation, VenueCapacity, ImageURL)
VALUES ('The garden venue', 'Muldersdrift', 60, 'https://www.iiemsa.co.za/'),
('The Lillies venue', 'Muldersdrift', 60, 'https://wp.presidio.gov/wp-content/uploads/2023/07/0.3.4-Party-Venues.jpg');

SELECT * FROM Venues;

--TABLE CREATION AND INSERTION FOR Events
CREATE TABLE Events(
	EVENTID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	EventName VARCHAR(250) NOT NULL,
	EventDate DATE NOT NULL,
	Descript VARCHAR(250) NOT NULL,
	VENUEID INT NOT NULL,
	FOREIGN KEY(VENUEID) REFERENCES Venues(VENUEID)
);

INSERT INTO Events(EventName, EventDate, Descript, VENUEID)
VALUES ('Birthday', '2024-07-22', '21st birthday party celebration.', 1),
('Wedding', '2024-08-16', 'Newly Weds celebarion.', 2);

SELECT * FROM Events;

--TABLE CREATION AND INSERTION FOR Booking
CREATE TABLE Bookings(
	BOOKINGID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	BookingDate DATE NOT NULL,
	VENUEID INT NOT NULL,
	EVENTID INT NOT NULL,
	FOREIGN KEY(VENUEID) REFERENCES Venues(VENUEID),
	FOREIGN KEY(EVENTID) REFERENCES Events(EVENTID)
);

INSERT INTO Bookings(BookingDate, VENUEID, EVENTID)
VALUES ('2024-07-4', 1, 1),
('2024-05-23', 2, 2);

SELECT * FROM Bookings;

ALTER TABLE Venues
ALTER COLUMN ImageURL NVARCHAR(500);

SELECT * FROM Venues;

