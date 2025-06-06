import { h } from 'vue';
import { NAvatar, NTag } from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {Client, Subject} from "@/api/globals";
import ClientGroupIndicator from "@/components/ClientGroupIndicator/index.vue";

export const columns: BasicColumn<Subject>[] = [
  {
    title: '昵称',
    key: 'name',
  },
  {
    title: '用户名',
    key: 'userName'
  },
  {
    title: '创建时间',
    key: 'createdTime'
  }
];
