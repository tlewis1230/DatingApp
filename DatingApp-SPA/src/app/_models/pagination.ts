export interface Pagination {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

// use for list of users and Messages - make it generic
export class PaginatedResult<T> {
    result: T;
    pagination: Pagination;
}
