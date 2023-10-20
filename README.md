# ApplicationSentinel
This is an API project which helps with encryption and decryption of strings, it was created to hide secrets from app config files.

# SetUp
You will have to create a SQL table using below to get it working and also create an encrypted SQL server Connection String and add it to the Config file(one time activity)
CREATE TABLE [dbo].[EncryptionKeys](
	[EncryptionKey] [nchar](20) NOT NULL,
	[UserId] [nchar](30) NULL,
	[MachineNameOrIP] [nchar](40) NULL
) ON [PRIMARY]
