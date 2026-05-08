-- Script de criação da tabela utilizada pelo Projeto Conta Bancária.
-- Execute este arquivo no banco PostgreSQL configurado no appsettings.json.

CREATE TABLE IF NOT EXISTS contas (
    numero SERIAL PRIMARY KEY,
    agencia INTEGER NOT NULL CHECK (agencia > 0),
    tipo INTEGER NOT NULL CHECK (tipo IN (1, 2)),
    titular VARCHAR(120) NOT NULL,
    saldo REAL NOT NULL DEFAULT 0 CHECK (saldo >= -999999999),
    limite REAL NULL CHECK (limite IS NULL OR limite >= 0),
    aniversario INTEGER NULL CHECK (aniversario IS NULL OR aniversario BETWEEN 1 AND 31),
    criado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_contas_titular ON contas (titular);
CREATE INDEX IF NOT EXISTS idx_contas_tipo ON contas (tipo);

-- Dados opcionais para teste inicial:

-- INSERT INTO contas (agencia, tipo, titular, saldo, limite, aniversario)
-- VALUES
--     (1001, 1, 'Leticia Puga', 2050.00, 5000.00, NULL),
--     (1002, 2, 'Carlos Lima', 1200.00, NULL, 15);
