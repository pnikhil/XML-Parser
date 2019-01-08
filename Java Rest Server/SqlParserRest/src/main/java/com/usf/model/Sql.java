package com.usf.model;

import java.io.Serializable;
import java.util.List;

import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlRootElement;

@XmlRootElement(name = "sql")
@XmlAccessorType(XmlAccessType.FIELD)
public class Sql implements Serializable {

	private static final long serialVersionUID = 1L;
	private String sqlQuery;
	private int numberOfTables;
	private int numberOfColumns;
	private List<String> tableNames;
	private List<String> columnNames;
	private boolean isStoredProcedure;
	private String storedProcedure;
	private String operation;
	private String databaseType;
	
	public String getDatabaseType() {
		return databaseType;
	}

	public void setDatabaseType(String databaseType) {
		this.databaseType = databaseType;
	}

	public List<String> getTableNames() {
		return tableNames;
	}

	public void setTableNames(List<String> tableNames) {
		this.tableNames = tableNames;
	}

	public List<String> getColumnNames() {
		return columnNames;
	}

	public void setColumnNames(List<String> columnNames) {
		this.columnNames = columnNames;
	}
	
	public int getNumberOfTables() {
		return numberOfTables;
	}

	public void setNumberOfTables(int numberOfTables) {
		this.numberOfTables = numberOfTables;
	}

	public int getNumberOfColumns() {
		return numberOfColumns;
	}

	public void setNumberOfColumns(int numberOfColumns) {
		this.numberOfColumns = numberOfColumns;
	}

	public String getSqlQuery() {
		return sqlQuery;
	}

	public void setSqlQuery(String sqlQuery) {
		this.sqlQuery = sqlQuery;
	}

	public boolean isStoredProcedure() {
		return isStoredProcedure;
	}

	public void setStoredProcedure(boolean isStoredProcedure) {
		this.isStoredProcedure = isStoredProcedure;
	}

	public String getStoredProcedure() {
		return storedProcedure;
	}

	public void setStoredProcedure(String storedProcedure) {
		this.storedProcedure = storedProcedure;
	}

	public String getOperation() {
		return operation;
	}

	public void setOperation(String operation) {
		this.operation = operation;
	}

}
