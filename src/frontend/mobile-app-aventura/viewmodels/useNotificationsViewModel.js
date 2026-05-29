import { useMemo, useState } from 'react';

import {
  getNotificationData,
} from '../repositories/notificationRepository';

export default function useNotificationsViewModel() {
  const repositoryData = useMemo(
    () => getNotificationData(),
    []
  );

  const [sections, setSections] = useState(
    repositoryData.sections || []
  );

  function handleToggleOption(
    sectionId,
    optionId
  ) {
    setSections((previous) =>
      previous.map((section) => {
        if (section.id !== sectionId) {
          return section;
        }

        return {
          ...section,

          options: section.options.map(
            (option) => {
              if (option.id !== optionId) {
                return option;
              }

              return {
                ...option,
                enabled: !option.enabled,
              };
            }
          ),
        };
      })
    );
  }

  return {
    sections,
    handleToggleOption,
  };
}