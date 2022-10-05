import { ApiException, ValidationProblemDetails } from "app/generated/backend";

export function str2Hsl (source: string, saturation: number = 100, lightness: number = 60) {
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

export function getErrorMessage(e: ApiException | ValidationProblemDetails): string {
    if (e instanceof ValidationProblemDetails) {
        return Object.keys(e.errors).map(er => e.errors[er]).join(" ");
    }

    if (!ApiException.isApiException(e)) {
        return (e as ApiException).message;
    }

    const response = JSON.parse(e.response);

    if (response !== null) {
        
        return response["Message"];
    }

    return (e as ApiException).message;
}