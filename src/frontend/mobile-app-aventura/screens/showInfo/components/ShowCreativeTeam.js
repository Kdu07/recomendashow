import React, { useState, useRef } from 'react';
import { View, Text, TouchableOpacity, Animated, StyleSheet, Platform } from 'react-native';

import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing, radius } from '../../../constants/spacing';

export default function ShowCreativeTeam({ creativeTeam }) {
  const [isExpanded, setIsExpanded] = useState(false);
  const rotation = useRef(new Animated.Value(0)).current;

  function toggle() {
    const toValue = isExpanded ? 0 : 1;
    Animated.timing(rotation, {
      toValue,
      duration: 200,
      useNativeDriver: Platform.OS !== 'web',
    }).start();
    setIsExpanded((prev) => !prev);
  }

  const chevronRotation = rotation.interpolate({
    inputRange: [0, 1],
    outputRange: ['0deg', '180deg'],
  });

  return (
    <View style={styles.container}>
      <TouchableOpacity style={styles.card} onPress={toggle} activeOpacity={0.8}>
        <View style={styles.header}>
          <Text style={styles.sectionLabel}>{creativeTeam.sectionLabel}</Text>
          <Animated.Text
            style={[styles.chevron, { transform: [{ rotate: chevronRotation }] }]}
          >
            ▾
          </Animated.Text>
        </View>

        {isExpanded && (
          <View style={styles.list}>
            {creativeTeam.members.map((member) => (
              <Text key={`${member.role}-${member.name}`} style={styles.member}>
                <Text style={styles.name}>{member.name}: </Text>
                <Text style={styles.role}>{member.role}</Text>
              </Text>
            ))}
          </View>
        )}
      </TouchableOpacity>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: spacing.xl,
    paddingTop: spacing.xl,
  },

  card: {
    backgroundColor: colors.whiteSoft,
    borderRadius: radius.md,
    padding: spacing.lg,
    gap: spacing.sm,
  },

  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
  },

  sectionLabel: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
    letterSpacing: 0.5,
  },

  chevron: {
    fontSize: typography.size.md,
    color: colors.purplePrimary,
  },

  list: {
    gap: spacing.xs,
  },

  member: {
    fontSize: typography.size.sm,
    lineHeight: typography.lineHeight.sm,
  },

  role: {
    fontWeight: typography.fontWeight.regular,
    color: colors.grayDark,
  },

  name: {
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },
});
