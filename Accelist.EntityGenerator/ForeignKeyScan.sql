SELECT
	obj.name AS ConstraintName,
	tbl.name AS TableName,
	col1.name AS ForeignKey,
	ref.name AS ReferencedTable,
	col2.name AS ReferencedKey
FROM sys.foreign_key_columns fkc
JOIN sys.objects obj ON obj.object_id = fkc.constraint_object_id
JOIN sys.tables tbl ON tbl.object_id = fkc.parent_object_id
JOIN sys.columns col1 ON col1.column_id = parent_column_id AND col1.object_id = tbl.object_id
JOIN sys.tables ref ON ref.object_id = fkc.referenced_object_id
JOIN sys.columns col2 ON col2.column_id = referenced_column_id AND col2.object_id = ref.object_id
