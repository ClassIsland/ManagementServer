import { h } from 'vue';
import { NAvatar, NTag } from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {Client} from "@/api/globals";

export const columns: BasicColumn<Client>[] = [
  {
    title: 'ID',
    key: 'id',
  },
  {
    title: 'CUID',
    key: 'cuid',
  },
  {
    title: '注册时间',
    key: 'registerTime'
  }
];
