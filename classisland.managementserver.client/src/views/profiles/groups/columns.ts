import { h } from 'vue';
import { NAvatar, NTag } from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {Client, ProfileGroup, Subject} from "@/api/globals";
import ClientGroupIndicator from "@/components/ClientGroupIndicator/index.vue";

export const columns: BasicColumn<ProfileGroup>[] = [
  {
    'title': '名称',
    'key': 'name',
    render(data) {
      return h(ClientGroupIndicator, {
        group: data,
      });
    }
  },
  {
    'title': '创建时间',
    'key': 'createdTime',
  },
];
