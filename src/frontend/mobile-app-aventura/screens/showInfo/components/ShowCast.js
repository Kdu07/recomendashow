import React from 'react';
import { View, Text, Image, ScrollView, StyleSheet } from 'react-native';

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
      <Text style={styles.memberName} numberOfLines={2}>{member.name}</Text>
      <Text style={styles.memberRole} numberOfLines={2}>{member.role}</Text>
    </View>
  );
}

export default function ShowCast({ cast }) {
  return (
    <View style={styles.container}>
      <Text style={styles.sectionLabel}>{cast.sectionLabel}</Text>

      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        contentContainerStyle={styles.list}
      >
        {cast.members.map((member) => (
          <CastMember key={member.id} member={member} />
        ))}
      </ScrollView>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    paddingTop: spacing.xl,
    gap: spacing.md,
  },

  sectionLabel: {
    paddingHorizontal: spacing.xl,
    fontSize: 17,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  list: {
    paddingHorizontal: spacing.xl,
    gap: spacing.lg,
  },

  member: {
    width: 80,
    alignItems: 'center',
    gap: spacing.xs,
  },

  avatar: {
    width: 64,
    height: 64,
    borderRadius: 32,
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

  memberName: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.semiBold,
    color: colors.grayDark,
    textAlign: 'center',
  },

  memberRole: {
    fontSize: typography.size.xs,
    fontWeight: typography.fontWeight.regular,
    color: colors.grayText,
    textAlign: 'center',
  },
});
