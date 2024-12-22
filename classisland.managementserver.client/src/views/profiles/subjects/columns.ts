import { h } from 'vue';
import { NAvatar, NTag } from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {Client, Subject} from "@/api/globals";

export const columns: BasicColumn<Subject>[] = [
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
    key: 'isOutDoor',
    render: (record) => {
      return h(NTag, { type: record.isOutDoor ? 'success' : 'default' }, { default: () => record.isOutDoor ? '是' : '否' });
    }
  }
];
