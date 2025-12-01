-- 1. CREATE DATABASE
CREATE DATABASE IF NOT EXISTS BankingSystem;
USE BankingSystem;

-- 2. CREATE TABLES

-- Table: Person (Parent Entity)
CREATE TABLE Person (
    person_id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    gender VARCHAR(20),
    age INT,
    identification VARCHAR(20) UNIQUE NOT NULL, -- National ID / Passport
    address VARCHAR(200),
    phone VARCHAR(20)
);

-- Table: Customer (Child Entity - Inheritance)
CREATE TABLE Customer (
    customer_id INT AUTO_INCREMENT PRIMARY KEY,
    password VARCHAR(50) NOT NULL,
    status BOOLEAN DEFAULT TRUE, -- TRUE = Active
    person_id INT NOT NULL,
    
    CONSTRAINT FK_Customer_Person FOREIGN KEY (person_id) 
    REFERENCES Person(person_id)
    ON DELETE CASCADE 
    ON UPDATE CASCADE
);

-- Table: Account
CREATE TABLE Account (
    account_number INT PRIMARY KEY, -- Unique ID defined by business logic
    account_type VARCHAR(20) NOT NULL, -- Savings, Checking
    initial_balance DECIMAL(10, 2) DEFAULT 0.00,
    status BOOLEAN DEFAULT TRUE,
    customer_id INT NOT NULL,
    
    CONSTRAINT FK_Account_Customer FOREIGN KEY (customer_id)
    REFERENCES Customer(customer_id)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
);

-- Table: Transaction (formerly Movimientos)
CREATE TABLE Transaction (
    transaction_id INT AUTO_INCREMENT PRIMARY KEY,
    date DATETIME DEFAULT CURRENT_TIMESTAMP,
    transaction_type VARCHAR(30) NOT NULL, -- Deposit, Withdrawal
    amount DECIMAL(10, 2) NOT NULL,
    balance DECIMAL(10, 2) NOT NULL, -- Post-transaction balance
    account_number INT NOT NULL,
    
    CONSTRAINT FK_Transaction_Account FOREIGN KEY (account_number)
    REFERENCES Account(account_number)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
);

-- 3. SEED DATA (Testing)

-- Create a Person
INSERT INTO Person (name, gender, age, identification, address, phone)
VALUES ('Jose Lema', 'Male', 30, '1234567890', 'Otavalo sn y principal', '098254785');

-- Create the Customer linked to the Person
INSERT INTO Customer (password, status, person_id)
VALUES ('1234', TRUE, 1);

-- Create an Account for the Customer
INSERT INTO Account (account_number, account_type, initial_balance, status, customer_id)
VALUES (478758, 'Savings', 2000.00, TRUE, 1);

-- Register Transactions
INSERT INTO Transaction (date, transaction_type, amount, balance, account_number)
VALUES 
(NOW(), 'Withdrawal', -575.00, 1425.00, 478758),
(NOW(), 'Deposit', 600.00, 2025.00, 478758);

-- 4. QUERY
SELECT 
    p.name, 
    c.customer_id, 
    a.account_number, 
    a.account_type, 
    t.transaction_type, 
    t.amount, 
    t.balance
FROM Person p
JOIN Customer c ON p.person_id = c.person_id
JOIN Account a ON c.customer_id = a.customer_id
JOIN Transaction t ON a.account_number = t.account_number;