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

-- ==========================================
-- 1. CREACIÓN DE USUARIOS (Personas y Clientes)
-- ==========================================

-- Insertar Jose Lema
INSERT INTO Person (name, gender, age, identification, address, phone) 
VALUES ('Jose Lema', 'Masculino', 30, 'ID-JOSE001', 'Otavalo sn y principal', '098254785');

INSERT INTO Customer (password, status, person_id) 
VALUES ('1234', TRUE, (SELECT person_id FROM Person WHERE identification = 'ID-JOSE001'));

-- Insertar Marianela Montalvo
INSERT INTO Person (name, gender, age, identification, address, phone) 
VALUES ('Marianela Montalvo', 'Femenino', 28, 'ID-MARIA002', 'Amazonas y NNUU', '097548965');

INSERT INTO Customer (password, status, person_id) 
VALUES ('5678', TRUE, (SELECT person_id FROM Person WHERE identification = 'ID-MARIA002'));

-- Insertar Juan Osorio
INSERT INTO Person (name, gender, age, identification, address, phone) 
VALUES ('Juan Osorio', 'Masculino', 35, 'ID-JUAN003', '13 junio y Equinoccial', '098874587');

INSERT INTO Customer (password, status, person_id) 
VALUES ('1245', TRUE, (SELECT person_id FROM Person WHERE identification = 'ID-JUAN003'));


-- ==========================================
-- 2. CREACIÓN DE CUENTAS (Estado Inicial)
-- ==========================================

-- Cuenta 478758 para Jose Lema (Ahorro, 2000)
INSERT INTO Account (account_number, account_type, initial_balance, status, customer_id) 
VALUES (478758, 'Ahorro', 2000.00, TRUE, 
    (SELECT c.customer_id FROM Customer c JOIN Person p ON c.person_id = p.person_id WHERE p.name = 'Jose Lema'));

-- Cuenta 225487 para Marianela Montalvo (Corriente, 100)
INSERT INTO Account (account_number, account_type, initial_balance, status, customer_id) 
VALUES (225487, 'Corriente', 100.00, TRUE, 
    (SELECT c.customer_id FROM Customer c JOIN Person p ON c.person_id = p.person_id WHERE p.name = 'Marianela Montalvo'));

-- Cuenta 495878 para Juan Osorio (Ahorros, 0)
INSERT INTO Account (account_number, account_type, initial_balance, status, customer_id) 
VALUES (495878, 'Ahorro', 0.00, TRUE, 
    (SELECT c.customer_id FROM Customer c JOIN Person p ON c.person_id = p.person_id WHERE p.name = 'Juan Osorio'));

-- Cuenta 496825 para Marianela Montalvo (Ahorros, 540)
INSERT INTO Account (account_number, account_type, initial_balance, status, customer_id) 
VALUES (496825, 'Ahorro', 540.00, TRUE, 
    (SELECT c.customer_id FROM Customer c JOIN Person p ON c.person_id = p.person_id WHERE p.name = 'Marianela Montalvo'));


-- ==========================================
-- 3. CREAR NUEVA CUENTA PARA JOSE LEMA
-- ==========================================

-- Cuenta 585545 para Jose Lema (Corriente, 1000)
INSERT INTO Account (account_number, account_type, initial_balance, status, customer_id) 
VALUES (585545, 'Corriente', 1000.00, TRUE, 
    (SELECT c.customer_id FROM Customer c JOIN Person p ON c.person_id = p.person_id WHERE p.name = 'Jose Lema'));


-- ==========================================
-- 4. REGISTRAR MOVIMIENTOS (Transactions)
-- ==========================================
-- Nota: El campo 'balance' representa el saldo DESPUÉS de la transacción.
-- El campo 'amount' lo coloco en negativo si es retiro, positivo si es depósito, para consistencia matemática.

-- Movimiento 1: Cuenta 478758 (Jose). Saldo Inicial: 2000. Retiro de 575.
-- Nuevo Saldo = 1425
INSERT INTO Transaction (date, transaction_type, amount, balance, account_number) 
VALUES (NOW(), 'Retiro', 575.00, 1425.00, 478758);

-- Movimiento 2: Cuenta 225487 (Marianela). Saldo Inicial: 100. Depósito de 600.
-- Nuevo Saldo = 700
INSERT INTO Transaction (date, transaction_type, amount, balance, account_number) 
VALUES (NOW(), 'Deposito', 600.00, 700.00, 225487);

-- Movimiento 3: Cuenta 495878 (Juan). Saldo Inicial: 0. Depósito de 150.
-- Nuevo Saldo = 150
INSERT INTO Transaction (date, transaction_type, amount, balance, account_number) 
VALUES (NOW(), 'Deposito', 150.00, 150.00, 495878);

-- Movimiento 4: Cuenta 496825 (Marianela). Saldo Inicial: 540. Retiro de 540.
-- Nuevo Saldo = 0
INSERT INTO Transaction (date, transaction_type, amount, balance, account_number) 
VALUES (NOW(), 'Retiro', 540.00, 0.00, 496825);