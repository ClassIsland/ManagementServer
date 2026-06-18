import { Alova } from '@/utils/http/alova/index';

export interface ClientAutomationConfig {
  clientCuid: string;
  workflowsJson: string;
  createdTime: string;
  updatedTime: string;
}

export function getClientAutomation(cuid: string) {
  return Alova.Get<ClientAutomationConfig>(`/api/v1/clients/${cuid}/automation`);
}

export function updateClientAutomation(cuid: string, data: { workflowsJson?: string }) {
  return Alova.Put<ClientAutomationConfig>(`/api/v1/clients/${cuid}/automation`, data);
}

export function requestAutomationUpload(cuid: string) {
  return Alova.Post(`/api/v1/clients/${cuid}/automation/request-upload`);
}
