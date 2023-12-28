exec sp_MSforeachtable "declare @name nvarchar(max); set @name = parsename('?', 1); exec sp_MSdropconstraints @name";
exec sp_MSforeachtable "drop table ?";


drop table if exists [User]
drop table if exists [Container]
drop table if exists [File]
drop table if exists [Role]
drop table if exists [UserRole]
drop table if exists [Authorization]
drop table if exists [RoleAuhorization]
drop table if exists [Configuration]

CREATE TABLE [User] (
	UserID int IDENTITY(1,1) NOT NULL,
	UserName nvarchar(100) NOT NULL,
	Password nvarchar(50) NOT NULL,
	Email nvarchar(100) NULL,
	LastModified datetime NOT NULL,
	ModifiedUserId int NULL,
	constraint PK_User_UserID primary key (UserID)
)

CREATE TABLE [Container] (
	ContainerID int IDENTITY(1,1) NOT NULL,
	ContainerName nvarchar(50) NOT NULL,
	LastModified datetime NOT NULL,
	ModifiedUserId int NULL,
	UserID int NOT NULL,
	constraint PK_Container_ContainerID primary key (ContainerID),
	constraint FK_Container_UserID foreign key (UserID) references [User] (UserID)
	ON UPDATE CASCADE
)

CREATE TABLE [File] (
	FileID int IDENTITY(1,1) NOT NULL,
	FileName nvarchar(50) NOT NULL,
	Type nvarchar(20) NOT NULL,
	Size decimal(10, 2) NOT NULL,
	IsDeleted bit NOT NULL,
	isPublic bit NOT NULL,
	Description nvarchar(500) NULL,
	LastModified datetime NOT NULL,
	ModifiedUserID int NULL,
	ContainerID int NOT NULL,
	GUID varchar(100) NOT NULL,
	constraint PK_File_FileID primary key (FileID),
	constraint FK_File_ContainerID foreign key (ContainerID) references [Container] (ContainerID)
	ON UPDATE CASCADE
	ON DELETE CASCADE
)

CREATE TABLE [Role] (
	RoleID int IDENTITY(1,1) NOT NULL,
	RoleName nvarchar(20) NOT NULL,
	LastModified datetime NOT NULL,
	ModifiedUserID int NULL,
	constraint PK_Role_RoleID primary key (RoleID)
)

CREATE TABLE [UserRole] (
	UserID int NOT NULL,
	RoleID int NOT NULL,
	constraint PK_UserRole_UserIDRoleID primary key (UserID, RoleID),
	constraint FK_UserRole_UserID foreign key (UserID) references [User] (UserID)
	ON UPDATE CASCADE
	ON DELETE CASCADE,
	constraint FK_UserRole_RoleID foreign key (RoleID) references [Role] (RoleID)
	ON UPDATE CASCADE
	ON DELETE CASCADE
)

CREATE TABLE [Authorization] (
	AuthorizationID int IDENTITY(1,1) NOT NULL,
	AuthorizationName nvarchar(20) NOT NULL,
	LastModified datetime NOT NULL,
	ModifiedUserID int NULL,
	constraint PK_Authorization_AuthorizationID primary key (AuthorizationID)
)

CREATE TABLE [RoleAuthorization] (
	AuthorizationID int NOT NULL,
	RoleID int NOT NULL,
	constraint PK_RoleAuthorization_AuthorizationIDRoleID primary key (AuthorizationID, RoleID),
	constraint FK_RoleAuthorization_AuthorizationID foreign key (AuthorizationID) references [Authorization] (AuthorizationID)
	ON UPDATE CASCADE
	ON DELETE CASCADE,
	constraint FK_RoleAuthorization_RoleID foreign key (RoleID) references [Role] (RoleID)
	ON UPDATE CASCADE
	ON DELETE CASCADE
)

CREATE TABLE [Configuration] (
	ConfigID int IDENTITY(1,1) NOT NULL,
	ConfigName nvarchar(20) NOT NULL,
	LastModified datetime NOT NULL,
	ModifiedUserID int NULL,
	ConfigValue varchar(500) NULL,
	constraint PK_Configuration_ConfigID primary key (ConfigID)
)

CREATE TABLE [RefreshToken] (
	TokenID int IDENTITY(1,1) NOT NULL,
	Token nvarchar(500) NOT NULL,
	IsActive bit NOT NULL,
	UserID int NOT NULL,
	constraint PK_RefreshToken_tokenID primary key (TokenID),
	constraint FK_RefreshToken_UserID foreign key (UserID) references [User] (UserID)
	ON UPDATE CASCADE
	ON DELETE CASCADE
)

insert [User] (UserName, Password, Email, LastModified) values ('adminuser', 'admin', 'admin@ceridian.com', GETDATE())
insert [User] (UserName, Password, Email, LastModified) values ('normaluser', 'user', 'user@ceridian.com', GETDATE())
-- insert [RefreshToken] (Token, IsActive, UserID) values ('CqPmZl3u+W9ttPIJxIvBm3WH8rk9Yvo9rFQGO+ho4fs=', 1, 1)
insert [Role] (RoleName, LastModified) values ('admin', GETDATE())
insert [UserRole] values (1, 1)
insert [Role] (RoleName, LastModified) values ('user', GETDATE())
insert [UserRole] values (2, 2)
insert [Container] (ContainerName, LastModified, ModifiedUserId, UserID) values ('adminuser', GETDATE(), 1, 1)
insert [Configuration] (ConfigName, LastModified, ModifiedUserID, ConfigValue) values ('maxSize', GETDATE(), 1, '20')
insert [Configuration] (ConfigName, LastModified, ModifiedUserID, ConfigValue) values ('allowedTypes', GETDATE(), 1, '"["all"]"')

select * from [User]
select * from [Role]
select * from [UserRole]
select * from [RefreshToken]
select * from [Container]
select * from [File]
select * from [RoleAuthorization]
select * from [Configuration]
select * from [Authorization]

delete from [User] where UserID in (2, 3)
delete from [Configuration] where ConfigValue = 'all'