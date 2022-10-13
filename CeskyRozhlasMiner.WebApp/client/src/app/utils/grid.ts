import { GridComparatorFn, GridValueFormatterParams } from "@mui/x-data-grid";
import dayjsAsUtc from "./dayjsAsUtc";
import { useDateFormat, useDateTimeFormat } from "./localization";

export const gridPageSizes: number[] = [10, 20, 50, 100];

export const dateValueFormatter = (params: GridValueFormatterParams<Date>): string => {
    return dateFormatter(params.value);
};

export const dateTimeValueFormatter = (params: GridValueFormatterParams<Date>): string => {
    return dateTimeFormatter(params.value);
};

export const dateFormatter = (d: Date): string => {
    return dayjsAsUtc(d).local().format(useDateFormat);
};

export const dateTimeFormatter = (d: Date): string => {
    return dayjsAsUtc(d).local().format(useDateTimeFormat);
};

export const dateTimeSortComparator: GridComparatorFn = (v1: Date, v2: Date): number => {
    return v2.getMilliseconds() - v1.getMilliseconds();
};