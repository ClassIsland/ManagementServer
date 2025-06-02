import { h } from 'vue';
import { NAvatar, NTag } from 'naive-ui';
import ClientGroupIndicator from '@/components/ClientGroupIndicator/index.vue';
import { BasicColumn } from '@/components/Table';
import {AbstractClient, Client, ClientGroup} from "@/api/globals";

export const columns: BasicColumn<ClientGroup>[] = [
  {
    title: '名称',
    key: 'name',
    render(data) {
      return h(ClientGroupIndicator, {
        group: data,
      });
    }
  },
  {
    title: '创建时间',
    key: 'createdTime'
  }
];
