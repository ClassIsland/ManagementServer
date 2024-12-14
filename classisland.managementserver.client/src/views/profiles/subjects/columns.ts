import { h } from 'vue';
import { NAvatar, NTag } from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {Client} from "@/api/globals";

export const columns: BasicColumn<Client>[] = [
  {
    title: '名称',
    key: 'name',
  },
  {
    title: '简称',
    key: 'initials'
  },
  {
    title: '户外课程？',
    key: 'isOutDoor'
  }
];
