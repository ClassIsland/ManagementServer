import { h } from 'vue';
import { NAvatar, NTag } from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {ClassPlan, Client, ObjectsAssignee, Subject} from "@/api/globals";

export const columns: BasicColumn<ObjectsAssignee>[] = [
  {
    title: '目标',
    key: 'id',
    render: (record) => {
      switch (record.assigneeType) {
        case 1:
          return record.targetClientCuid;
        case 2:
          return record.targetClientId;
        case 3:
          return record.targetGroup?.name;
      }
      return "？？？"
    }
  },
  {
    title: '类型',
    key: 'assigneeType',
    render: (record) => {
      switch (record.assigneeType) {
        case 1:
          return "CUID";
        case 2:
          return "客户端 ID";
        case 3:
          return "客户端组";
      }
      return "？？？"
    }
  }
];
