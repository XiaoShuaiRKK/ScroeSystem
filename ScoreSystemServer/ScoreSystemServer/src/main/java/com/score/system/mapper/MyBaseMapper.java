package com.score.system.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;

import java.util.List;

public interface MyBaseMapper<T> extends BaseMapper<T> {
    int insertBatchSomeColumn(List<T> entityList);
}
