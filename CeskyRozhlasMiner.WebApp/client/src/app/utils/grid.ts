import { GridComparatorFn, GridValueFormatterParams } from "@mui/x-data-grid";

export const gridPageSizes: number[] = [10, 20, 50, 100];

export const dateTimeValueFormatter = (params: GridValueFormatterParams<string>): string => {
    return params.value.toLocaleString();
};

export const dateTimeSortComparator: GridComparatorFn = (v1: Date, v2: Date): number => {
    return v2.getMilliseconds() - v1.getMilliseconds();
};
