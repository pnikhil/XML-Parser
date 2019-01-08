package com.usf.app;

import org.glassfish.jersey.server.ResourceConfig;

public class StartPoint extends ResourceConfig {
    public StartPoint() {
        // Define the package which contains the service classes.
        packages("com.usf.service");
    }
}
