create procedure spDelete
(
	@id int,
	@tabela varchar(max)
)

as
begin
	declare @sql varchar(max);
	set @sql = ' delete ' + @tabela +
	' where id = ' + cast(@id as varchar(max))
	exec(@sql)
end
GO

create procedure spConsulta
(
	@id int,
	@tabela varchar(max)
)
as
begin
	declare @sql varchar(max);
	set @sql = 'select * from ' + @tabela +
	' where id = ' + cast(@id as varchar(max))
	exec(@sql)
end
GO

create procedure spListagem
(
	@tabela varchar(max),
	@ordem varchar(max)
)
as
begin
	exec('select * from ' + @tabela +
	' order by ' + @ordem)
end
GO

create procedure spProximoId
(
	@tabela varchar(max)
)
as
begin
	exec('select isnull(max(id) +1, 1) as MAIOR from ' +@tabela)
end
GO


--estado
CREATE PROCEDURE spInsert_Estado (
	@id int,
	@descricao VARCHAR(25)
)
AS
BEGIN
  INSERT INTO Estado (Id, Descricao) VALUES (@id, @descricao);
END
GO

CREATE PROCEDURE spUpdate_Estado (
	@id INT, 
	@descricao VARCHAR(25)
)
AS
BEGIN
  UPDATE Estado SET Descricao = @descricao WHERE Id = @id;
END
GO

--Categoria
CREATE PROCEDURE spInsert_Categoria (
	@id int,
	@descricao VARCHAR(25)
)
AS
BEGIN
  INSERT INTO Categoria (Id, Descricao) VALUES (@id, @descricao);
END
GO

CREATE PROCEDURE spUpdate_Categoria (
	@id INT,
	@descricao VARCHAR(25)
)
AS
BEGIN
  UPDATE Categoria SET Descricao = @descricao WHERE Id = @id;
END
GO

--empresa
CREATE PROCEDURE spInsert_Empresa (
	@id int,
  @descricao VARCHAR(50),
  @categoriaId INT,
  @estadoId INT,
  @dataFundacao DATETIME,
  @imagem varbinary(MAX)
)
AS
BEGIN
  INSERT INTO Empresa (Id, Descricao, CategoriaId, EstadoId, DataFundacao, Imagem)
  VALUES (@id, @descricao, @categoriaId, @estadoId, @dataFundacao, @imagem);
END
GO

CREATE PROCEDURE spUpdate_Empresa (
  @id INT,
  @descricao VARCHAR(50),
  @categoriaId INT,
  @estadoId INT,
  @dataFundacao DATETIME,
  @imagem varbinary(MAX)
)
AS
BEGIN
  UPDATE Empresa
  SET Descricao = @descricao,
      CategoriaId = @categoriaId,
      EstadoId = @estadoId,
      DataFundacao = @dataFundacao,
      Imagem = @imagem
  WHERE Id = @id;
END
GO

--unidade
CREATE PROCEDURE spInsert_Unidade (
	@Id int,
  @descricao VARCHAR(50),
  @empresaId INT,
  @estadoId INT,
  @dataFundacao DATETIME
)
AS
BEGIN
  INSERT INTO Unidade (Id, Descricao, EmpresaId, EstadoId, DataFundacao)
  VALUES (@Id, @descricao, @empresaId, @estadoId, @dataFundacao);
END
GO

CREATE PROCEDURE spUpdate_Unidade (
  @id INT,
  @descricao VARCHAR(50),
  @empresaId INT,
  @estadoId INT,
  @dataFundacao DATETIME
)
AS
BEGIN
  UPDATE Unidade
  SET Descricao = @descricao,
      EmpresaId = @empresaId,
      EstadoId = @estadoId,
      DataFundacao = @dataFundacao
  WHERE Id = @id;
END
GO

--dispositivo
CREATE PROCEDURE spInsert_Dispositivo (
  @id INT,
  @modelo VARCHAR(50),
  @unidadeId INT,
  @dataInstalacao DATETIME,
  @imagem varbinary(MAX)
)
AS
BEGIN
  INSERT INTO Dispositivo (Id, Modelo, UnidadeId, DataInstalacao, Imagem)
  VALUES (@id, @modelo, @unidadeId, @dataInstalacao, @imagem);
END
GO

CREATE PROCEDURE spUpdate_Dispositivo (
  @id INT,
  @modelo VARCHAR(50),
  @unidadeId INT,
  @dataInstalacao DATETIME,
  @imagem varbinary(MAX)
)
AS
BEGIN
  UPDATE Dispositivo
  SET Modelo = @modelo,
      UnidadeId = @unidadeId,
      DataInstalacao = @dataInstalacao,
      Imagem = @imagem
  WHERE Id = @id;
END
GO

--listar dispositivo por unidade
CREATE PROCEDURE spListagem_DispositivoPorUnidade(
    @UnidadeId INT
)
AS
BEGIN
    SELECT
        d.Id,
        d.Modelo,
        d.DataInstalacao,
        d.Imagem,
		u.Id AS unidadeId,
        u.Descricao AS unidadeNome
    FROM Dispositivo d
    JOIN Unidade u ON d.UnidadeId = u.Id
    WHERE d.UnidadeId = @UnidadeId;
END
GO


--listar unidades por empresa
CREATE PROCEDURE spListagem_UnidadesPorEmpresa 
(	
	@empresaId int
)
AS
BEGIN
  SELECT
    u.Id,
    u.Descricao AS UnidadeNome,
	e.Id AS EmpresaId,
    e.Descricao AS EmpresaNome,
	es.Id AS EstadoId,
    es.Descricao AS EstadoNome,
    u.DataFundacao
  FROM Unidade u
  JOIN Empresa e ON u.EmpresaId = e.Id
  JOIN Estado es ON u.EstadoId = es.Id
  WHERE u.EmpresaId = @empresaId;
END
GO

--lista empresas
CREATE PROCEDURE spListagem_Empresas
(
    @tabela nvarchar(128),
    @ordem nvarchar(128)
)
AS
BEGIN
    DECLARE @sqlCommand nvarchar(max);

    SET @sqlCommand = N'
    SELECT 
        e.Id,
        e.Descricao,
		c.Id AS CategoriaId,
        c.Descricao AS CategoriaNome,
		es.Id AS EstadoId,
        es.Descricao AS EstadoNome,
        e.DataFundacao,
        e.Imagem
    FROM 
        ' + QUOTENAME(@tabela) + N' e
    JOIN 
        Categoria c ON e.CategoriaId = c.Id
    JOIN 
        Estado es ON e.EstadoId = es.Id
    ORDER BY ' + @ordem;

    EXEC sp_executesql @sqlCommand;
END;
GO

create procedure spListagemAvancada_Empresas(
    @descricao NVARCHAR(50) = NULL,
    @categoriaId INT = 0,
    @estadoId INT = 0
)
AS
BEGIN
    SELECT 
        e.Id,
        e.Descricao,
        e.CategoriaId,
        c.Descricao AS CategoriaNome,
        e.EstadoId,
        es.Descricao AS EstadoNome,
        e.DataFundacao,
        e.Imagem
    FROM Empresa e
    INNER JOIN Categoria c ON e.CategoriaId = c.Id
    INNER JOIN Estado es ON e.EstadoId = es.Id
    WHERE 
        (@descricao IS NULL OR @descricao = '' OR e.Descricao LIKE '%' + @descricao + '%') AND
        (@categoriaId = 0 OR e.CategoriaId = @categoriaId) AND
        (@estadoId = 0 OR e.EstadoId = @estadoId)
END
GO

CREATE PROCEDURE spListagemAvancada_Unidades(
    @descricao NVARCHAR(50) = NULL,
    @empresaId INT = 0,
    @categoriaId INT = 0,
    @estadoId INT = 0
)
AS
BEGIN
    SELECT 
        u.Id,
        u.Descricao,
        u.EmpresaId,
        e.Descricao AS EmpresaNome,
        e.CategoriaId,
        c.Descricao AS CategoriaNome,
        u.EstadoId,
        es.Descricao AS EstadoNome,
        u.DataFundacao
    FROM Unidade u
    INNER JOIN Empresa e ON u.EmpresaId = e.Id
    INNER JOIN Categoria c ON e.CategoriaId = c.Id
    INNER JOIN Estado es ON u.EstadoId = es.Id
    WHERE 
        (@descricao IS NULL OR @descricao = '' OR u.Descricao LIKE '%' + @descricao + '%') AND
        (@empresaId = 0 OR u.EmpresaId = @empresaId) AND
        (@categoriaId = 0 OR e.CategoriaId = @categoriaId) AND
        (@estadoId = 0 OR u.EstadoId = @estadoId)
END
GO


--ok
CREATE PROCEDURE spListagemAvancada_Dispositivos
    @descricao NVARCHAR(50) = NULL,
    @empresaId INT = 0,
    @unidadeId INT = 0,
    @categoriaId INT = 0,
    @estadoId INT = 0
AS
BEGIN
    SELECT 
        d.Id,
        d.Modelo,
        d.UnidadeId,
        u.Descricao AS UnidadeNome,
        u.EmpresaId,
        e.Descricao AS EmpresaNome,
        e.CategoriaId,
        c.Descricao AS CategoriaNome,
        u.EstadoId,
        es.Descricao AS EstadoNome,
        d.DataInstalacao,
        d.Imagem
    FROM Dispositivo d
    INNER JOIN Unidade u ON d.UnidadeId = u.Id
    INNER JOIN Empresa e ON u.EmpresaId = e.Id
    INNER JOIN Categoria c ON e.CategoriaId = c.Id
    INNER JOIN Estado es ON u.EstadoId = es.Id
    WHERE 
        (@descricao IS NULL OR @descricao = '' OR d.Modelo LIKE '%' + @descricao + '%') AND
        (@empresaId = 0 OR e.Id = @empresaId) AND
        (@unidadeId = 0 OR u.Id = @unidadeId) AND
        (@categoriaId = 0 OR c.Id = @categoriaId) AND
        (@estadoId = 0 OR es.Id = @estadoId)
END
GO


--ok
CREATE PROCEDURE spListagem_DispositivosInfo
AS
BEGIN
    SELECT 
        d.Id,
        u.Id AS UnidadeId,
		u.Descricao AS UnidadeNome,
		e.Id AS EmpresaId,
        e.Descricao AS EmpresaNome,
		c.Id AS CategoriaId,
        c.Descricao AS CategoriaNome,
		es.Id AS EstadoId,
        es.Descricao AS EstadoNome
    FROM 
        Dispositivo d
    INNER JOIN 
        Unidade u ON d.UnidadeId = u.Id
    INNER JOIN 
        Empresa e ON u.EmpresaId = e.Id
    INNER JOIN 
        Categoria c ON e.CategoriaId = c.Id
    INNER JOIN 
        Estado es ON u.EstadoId = es.Id
END
GO



