import { Alova } from '@/utils/http/alova/index';

export interface ComponentTemplate {
  id: string;
  name: string;
  layoutJson: string;
  pluginComponentTypes: string;
  createdTime: string;
  updatedTime: string;
}

export interface ClientComponentConfig {
  clientCuid: string;
  templateId: string | null;
  layoutJson: string;
  overrideJson: string | null;
  pluginComponentTypes: string | null;
  createdTime: string;
  updatedTime: string;
}

export function listTemplates() {
  return Alova.Get<ComponentTemplate[]>('/api/v1/component-templates');
}

export function getTemplate(id: string) {
  return Alova.Get<ComponentTemplate>(`/api/v1/component-templates/${id}`);
}

export function createTemplate(data: Partial<ComponentTemplate>) {
  return Alova.Post<ComponentTemplate>('/api/v1/component-templates', data);
}

export function updateTemplate(id: string, data: Partial<ComponentTemplate>) {
  return Alova.Put<ComponentTemplate>(`/api/v1/component-templates/${id}`, data);
}

export function deleteTemplate(id: string) {
  return Alova.Delete(`/api/v1/component-templates/${id}`);
}

export function assignTemplate(id: string, clientCuids: string[]) {
  return Alova.Post(`/api/v1/component-templates/${id}/assign`, { clientCuids });
}

export function getClientComponents(cuid: string) {
  return Alova.Get<ClientComponentConfig>(`/api/v1/clients/${cuid}/components`);
}

export function updateClientComponents(cuid: string, data: { layoutJson?: string; overrideJson?: string }) {
  return Alova.Put<ClientComponentConfig>(`/api/v1/clients/${cuid}/components`, data);
}

export function requestClientUpload(cuid: string) {
  return Alova.Post(`/api/v1/clients/${cuid}/components/request-upload`);
}
