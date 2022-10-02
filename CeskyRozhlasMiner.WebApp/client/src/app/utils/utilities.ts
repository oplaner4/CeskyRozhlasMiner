import { ApiException } from "app/generated/backend";

/**
 * Returns the first value associated to the given search parameter.
 */
export function getUrlParamValue(param: string): string {
    const url = document.location.search || document.location.href;
    let urlParams = new URLSearchParams(url.substring(1));
    return urlParams.get(param) || '';
}

export function str2Hsl (source: string, saturation: number = 100, lightness: number = 75) {
    /**
     * Adapted from https://stackoverflow.com/a/21682946
     */

    let hash = 0;

    for (let i = 0; i < source.length; i++) {
      hash = source.charCodeAt(i) + ((hash << 5) - hash);
    }

    let h = hash % 360;
    return `hsl(${h}, ${saturation}%, ${lightness}%)`;
}

export function getInitials(source: string, limit: number): string[] {
    return source.split(/\s+/g).map(w => w[0]).filter((_, i) => i < limit);
}

export function getErrorMessage(ex: ApiException): string {
    let result = ex.message;
    const response = JSON.parse(ex.response);

    if (response !== null) {
        result = response["Message"];
    }

    return result;
}