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
    const currentSection = sections.find((section) => section.id === sectionId);
    const currentOption = currentSection?.options.find((option) => option.id === optionId);
    const changedOption = currentOption
      ? {
          title: currentOption.title,
          enabled: !currentOption.enabled,
        }
      : null;

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

    return changedOption;
  }

  return {
    sections,
    handleToggleOption,
  };
}
