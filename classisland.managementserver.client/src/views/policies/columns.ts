import { h } from 'vue';
import {NAvatar, NTag, NTime} from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {Client, Subject} from "@/api/globals";
import ClientGroupIndicator from "@/components/ClientGroupIndicator/index.vue";

export const columns: BasicColumn<Subject>[] = [
  {
    title: '名称',
    key: 'name',
  },
  {
    title: '创建时间',
    key: 'createdTime',
    render(data) {
      return h(NTime, { time:data.createdTime })
    },
  },
  {
    'title': '分组',
    'key': 'group',
    render(data) {
      return h(ClientGroupIndicator, {
        group: data.group,
      });
    }
  }
];
