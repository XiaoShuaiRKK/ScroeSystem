package com.score.system;

import org.mybatis.spring.annotation.MapperScan;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.transaction.annotation.EnableTransactionManagement;

@SpringBootApplication
@MapperScan("com.score.system.mapper")
@EnableTransactionManagement
public class ScoreSystemApplication {
    public static void main(String[] args) {
        SpringApplication.run(ScoreSystemApplication.class, args);
    }
}
