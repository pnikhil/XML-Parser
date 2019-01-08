package com.usf.restutil;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.stream.Collectors;

import com.usf.model.Sql;

import net.sf.jsqlparser.JSQLParserException;
import net.sf.jsqlparser.expression.Function;
import net.sf.jsqlparser.parser.CCJSqlParserUtil;
import net.sf.jsqlparser.schema.Column;
import net.sf.jsqlparser.schema.Table;
import net.sf.jsqlparser.statement.Statement;
import net.sf.jsqlparser.statement.select.TableFunction;
import net.sf.jsqlparser.util.TablesNamesFinder;

public class SqlUtil {

	static List<String> columnNames = new ArrayList<String>();

	public static Sql getSqlData(Sql sqlData) throws JSQLParserException {
		
		String sqlQuery = sqlData.getSqlQuery();
		sqlQuery = cleanSqlQuery(sqlQuery);
		
		Statement statement = CCJSqlParserUtil.parse(sqlQuery);

		TablesNamesFinder tablesNamesFinder = new TablesNamesFinder() {
			@Override
			public void visit(Column tableColumn) {
				columnNames.add(tableColumn.toString());
			}

			@Override
			public void visit(Table tableName) {
				
				super.visit(tableName);
			}

			@Override
			public void visit(TableFunction valuesList) {
				
				super.visit(valuesList);
			}
		};

		//System.out.println("all extracted tables=" + tablesNamesFinder.getTableList(statement));
		
		List<String> extractedTableNames = tablesNamesFinder.getTableList(statement);
		
		extractedTableNames.removeAll(Collections.singleton(null));
		columnNames = columnNames.stream().distinct().collect(Collectors.toList());
		
		Sql newSqlData = new Sql();
		newSqlData.setSqlQuery(sqlData.getSqlQuery());
		newSqlData.setTableNames(extractedTableNames);
		newSqlData.setColumnNames(columnNames);
		newSqlData.setNumberOfColumns(columnNames.size());
		newSqlData.setNumberOfTables(extractedTableNames.size());
		
		System.out.println(extractedTableNames);
		return newSqlData;

	}

	public static String cleanSqlQuery(String sqlQuery) {
		sqlQuery = sqlQuery.replaceAll("\\#\\[", "").replaceAll("\\]", "").replaceAll("\\[", "").replace("AT LOCAL", "");
		System.out.println(sqlQuery);
		return sqlQuery;
	}

	
	/*
	 * 
	 * UNCOMMENT THIS BLOCK FOR TESTING
	 * 
	public static void main(String[] args) throws JSQLParserException {
		Sql sql = new Sql();

		sql.setSqlQuery("");
		getSqlData(sql);
	}*/

}
