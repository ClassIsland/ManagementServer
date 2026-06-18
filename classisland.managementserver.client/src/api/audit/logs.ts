import { Alova } from '@/utils/http/alova/index';

export interface AuditLog {
  id: number;
  clientCuid: string;
  event: number;
  payload: string | null;
  eventTimeUtc: string;
  createdTime: string;
  updatedTime: string;
}

export interface PaginatedList<T> {
  pageIndex: number;
  totalPages: number;
  pageSize: number;
  itemCount: number;
  items: T[];
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export function listAuditLogs(params: { pageIndex?: number; pageSize?: number }) {
  return Alova.Get<PaginatedList<AuditLog>>('/api/v1/audit', { params });
}
