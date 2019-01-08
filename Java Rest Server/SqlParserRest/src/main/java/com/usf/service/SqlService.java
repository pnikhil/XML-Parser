package com.usf.service;

import java.io.IOException;

import javax.servlet.http.HttpServletResponse;
import javax.ws.rs.Consumes;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.MediaType;

import com.usf.model.Sql;
import com.usf.restutil.SqlUtil;

import net.sf.jsqlparser.JSQLParserException;

@Path("/service")
public class SqlService {
	
	@POST
	@Path("/getSqlData")
	@Consumes("application/json")
	@Produces({ MediaType.APPLICATION_JSON, MediaType.APPLICATION_XML })
	public Sql getSqlData(Sql sqlString, @Context HttpServletResponse servletResponse)
			throws IOException, JSQLParserException {		
		Sql sqlData = SqlUtil.getSqlData(sqlString);
		return sqlData;
	}

}
