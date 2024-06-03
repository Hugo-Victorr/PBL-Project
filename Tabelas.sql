use master

Create database PBL_Project

drop database PBL_Project

use PBL_Project

create table Estado(
	Id int NOT NULL PRIMARY KEY,
	Descricao varchar(25) NULL,
)

create table Categoria(
	Id int NOT NULL PRIMARY KEY,
	Descricao varchar(25) NULL,
)
	
create table Empresa(
	Id int NOT NULL PRIMARY KEY,
	Descricao varchar(50) NULL,
	CategoriaId int FOREIGN KEY REFERENCES Categoria(Id) NOT NULL,
	EstadoId int FOREIGN KEY REFERENCES Estado(Id) NOT NULL,
	DataFundacao datetime NULL,
	Imagem varbinary(MAX)
)

create table Unidade(
	Id int NOT NULL PRIMARY KEY,
	Descricao varchar(50) NULL,
	EmpresaId int FOREIGN KEY REFERENCES Empresa(Id) NOT NULL,
	EstadoId int FOREIGN KEY REFERENCES Estado(Id) NOT NULL,
	DataFundacao datetime NULL
)

create table Dispositivo(
	Id int NOT NULL PRIMARY KEY,
	Modelo varchar(50) NULL,
	UnidadeId int FOREIGN KEY REFERENCES Unidade(Id) NOT NULL,
	DataInstalacao datetime NOT NULL,
	Imagem varbinary(MAX)
)