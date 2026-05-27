import React from 'react';
import { View, Text, Image, StyleSheet } from 'react-native';

import { colors } from '../../../constants/colors';
import { typography } from '../../../constants/typography';
import { spacing } from '../../../constants/spacing';

function CastMember({ member }) {
  return (
    <View style={styles.member}>
      {member.image ? (
        <Image source={member.image} style={styles.avatar} resizeMode="cover" />
      ) : (
        <View style={[styles.avatar, styles.avatarPlaceholder]}>
          <Text style={styles.avatarPlaceholderText}>+</Text>
        </View>
      )}
      <View style={styles.memberInfo}>
        <Text style={styles.memberName}>{member.name}</Text>
        <Text style={styles.memberRole}>{member.role}</Text>
      </View>
    </View>
  );
}

export default function ShowCast({ cast }) {
  return (
    <View style={styles.container}>
      <View style={styles.divider} />

      <Text style={styles.sectionLabel}>{cast.sectionLabel}</Text>

      <View style={styles.list}>
        {cast.members.map((member) => (
          <CastMember key={member.id} member={member} />
        ))}
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingHorizontal: spacing.xl,
    paddingTop: spacing.lg,
    gap: spacing.md,
  },

  divider: {
    height: 1,
    backgroundColor: colors.grayLight,
    marginBottom: spacing.sm,
  },

  sectionLabel: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
    letterSpacing: 0.5,
  },

  list: {
    gap: spacing.md,
  },

  member: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },

  avatar: {
    width: 44,
    height: 44,
    borderRadius: 22,
  },

  avatarPlaceholder: {
    backgroundColor: colors.purplePrimary,
    alignItems: 'center',
    justifyContent: 'center',
  },

  avatarPlaceholderText: {
    fontSize: typography.size.lg,
    fontWeight: typography.fontWeight.bold,
    color: colors.white,
  },

  memberInfo: {
    flex: 1,
    gap: 2,
  },

  memberName: {
    fontSize: typography.size.sm,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
  },

  memberRole: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
  },
});
