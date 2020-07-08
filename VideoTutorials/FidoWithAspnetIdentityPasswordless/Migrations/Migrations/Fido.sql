CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

CREATE TABLE "FidoKeys" (
    "CredentialId" TEXT NOT NULL CONSTRAINT "PK_FidoKeys" PRIMARY KEY,
    "UserId" TEXT NULL,
    "UserHandle" TEXT NULL,
    "DisplayFriendlyName" TEXT NULL,
    "AttestationType" INTEGER NOT NULL,
    "AuthenticatorId" TEXT NULL,
    "AuthenticatorIdType" INTEGER NULL,
    "Counter" INTEGER NOT NULL,
    "KeyType" TEXT NULL,
    "Algorithm" TEXT NULL,
    "CredentialAsJson" TEXT NULL,
    "Created" TEXT NULL,
    "LastUsed" TEXT NULL
);

CREATE INDEX "IX_FidoKeys_UserId" ON "FidoKeys" ("UserId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200603171532_Fido', '3.1.4');

