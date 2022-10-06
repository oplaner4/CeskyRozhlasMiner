import { GridComparatorFn, GridValueFormatterParams } from "@mui/x-data-grid";
import moment from "moment";

export const gridPageSizes: number[] = [10, 20, 50, 100];

export const dateValueFormatter = (params: GridValueFormatterParams<Date>): string => {
    return dateFormatter(params.value);
};

export const dateTimeValueFormatter = (params: GridValueFormatterParams<Date>): string => {
    return dateTimeFormatter(params.value);
};

export const dateFormatter = (d: Date): string => {
    return moment(d).format("YYYY/MM/DD");
};

export const dateTimeFormatter = (d: Date): string => {
    return d.toUTCString();
};

export const dateTimeSortComparator: GridComparatorFn = (v1: Date, v2: Date): number => {
    return v2.getMilliseconds() - v1.getMilliseconds();
};
