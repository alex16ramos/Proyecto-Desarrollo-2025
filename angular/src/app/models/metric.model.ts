export interface ApiUsageMetricDto {
  id: string;
  endpoint: string;
  httpMethod: string;
  statusCode: number;
  executionDurationMs: number;
  userId?: string;
  userName?: string;
  ipAddress?: string;
  timestamp: string;
}

export interface GetApiUsageMetricsInput {
  endpoint?: string;
  httpMethod?: string;
  statusCode?: number;
  userId?: string;
  startDate?: string;
  endDate?: string;
  maxResultCount?: number;
  skipCount?: number;
}
