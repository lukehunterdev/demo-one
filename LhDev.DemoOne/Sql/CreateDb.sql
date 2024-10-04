CREATE TABLE User
(
    Id          INTEGER NOT NULL
                    CONSTRAINT User_PK
                    PRIMARY KEY autoincrement,
    Username    TEXT NOT NULL UNIQUE,
    Name        TEXT NOT NULL,
    CanEdit     TINYINT DEFAULT(FALSE) NOT NULL
);

CREATE TABLE Password
(
    UserId      INTEGER NOT NULL
                    CONSTRAINT Password_PK
                    PRIMARY KEY,
    Salt        TEXT NOT NULL,
    Hash        TEXT NOT NULL
);


CREATE TABLE Customer
(
    Id          INTEGER NOT NULL
                    CONSTRAINT Customer_PK
                    PRIMARY KEY autoincrement,
    FirstName   TEXT NOT NULL,
    Surname     TEXT NOT NULL
);
