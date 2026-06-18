import { Alova } from '@/utils/http/alova/index';

export interface GroupDistribution {
  groupName: string;
  count: number;
}

export interface AuditEventTrend {
  date: string;
  count: number;
}

export interface PolicyCoverage {
  totalClients: number;
  coveredClients: number;
  coverageRate: number;
}

export interface DashboardOverview {
  onlineCount: number;
  totalCount: number;
  groupDistribution: GroupDistribution[];
  auditEventTrend: AuditEventTrend[];
  versionDistribution: { version: string; count: number }[];
  policyCoverage: PolicyCoverage;
}

export function getDashboardOverview() {
  return Alova.Get<DashboardOverview>('/api/v1/dashboard/overview');
}
